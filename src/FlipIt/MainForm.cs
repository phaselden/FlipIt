/* Originally based on project by Frank McCown in 2010 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ScreenSaver
{
	enum DstIndicatorStyle
	{
		SmallDst,  // Small "DST" in top half, and day diff shown as -1d or +1d in bottom half
		DstAsAsterisk  // If day different, shows as mmm
	}
	
	public partial class MainForm : Form
	{
		#region Win32 API functions

		[DllImport("user32.dll")]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

		private const int SplitWidth = 4;
		private const int CityBoxSplitWidth = 2;
		private const int FontScaleFactor = 3;

        private readonly bool _display24HourTime;
        private readonly ScreenSetting _screenSetting;
		private Point _mouseLocation;
		private readonly bool _isPreviewMode;
		private readonly bool _showSeconds = false;
		private int _lastMinute = -1;
		private readonly int _fontSize = 350;
		private Font _primaryFont;
		private Font _cityFont;
		private Font _smallCityFont;
		private Font _primarySmallFont;
		private Graphics _graphics;
		private PrivateFontCollection _pfc;
		private FontFamily _fontFamily;

		private FontFamily FontFamily
        {
            get
            {
                if (_fontFamily == null)
                {
                    if (_pfc == null)
                    {
                        _pfc = InitFonts();
                    }
                    _fontFamily = _pfc.Families[0];
                }
                return _fontFamily ?? (_fontFamily = _pfc.Families[0]);
            }
        }

        private Graphics Gfx => _graphics ?? (_graphics = CreateGraphics());

		private Font PrimaryFont => _primaryFont ?? (_primaryFont = new Font(FontFamily, _fontSize, FontStyle.Bold));
		private Font PrimarySmallFont => _primarySmallFont ?? (_primarySmallFont = new Font(FontFamily, _fontSize / 9, FontStyle.Bold));

		private static readonly Color BackColorTop = Color.FromArgb(255, 15, 15, 15);
		private static readonly Color BackColorBottom = Color.FromArgb(255, 10, 10, 10);

		private readonly Brush _backFillTop = new SolidBrush(BackColorTop);
		private readonly Pen _cityBoxPen = new Pen(BackColorTop);
		private readonly Brush _backFillBottom = new SolidBrush(BackColorBottom);
		private readonly Brush _fontBrush = new SolidBrush(Color.FromArgb(255, 183, 183, 183));
		private readonly Pen _splitPen = new Pen(Color.Black, SplitWidth);
		private readonly Pen _smallSplitPen = new Pen(Color.Black, CityBoxSplitWidth);

		private const int BoxWidthPercentage = 70;
		private const int HorizontalGapBetweenBoxesPercent = 5;
		private const int VerticalGapBetweenBoxesPercent = 10;
		const int TimeLengthInChars = 9;
		
		DstIndicatorStyle _cityDisplayDstIndicatorStyle = DstIndicatorStyle.DstAsAsterisk; // .SmallDst;
		

		private List<City> _cities;
		private List<City> Cities => _cities ?? (_cities = GetCities());
		
		public MainForm()
		{
			InitializeComponent();
		}

		public MainForm(Rectangle bounds, bool display24HourTime, ScreenSetting screenSetting)
		{
            _display24HourTime = display24HourTime;
            _screenSetting = screenSetting;
			InitializeComponent();
			Bounds = bounds;
			_fontSize = bounds.Height / FontScaleFactor;
        }

		public MainForm(IntPtr previewWndHandle, bool display24HourTime, ScreenSetting screenSetting)
		{
            _display24HourTime = display24HourTime;
            _screenSetting = screenSetting;

			InitializeComponent();

			// Set the preview window as the parent of this window
			SetParent(Handle, previewWndHandle);

			// Make this a child window so it will close when the parent dialog closes
			SetWindowLong(Handle, -16, new IntPtr(GetWindowLong(Handle, -16) | 0x40000000));

			// Place our window inside the parent
			GetClientRect(previewWndHandle, out var parentRect);
			Size = parentRect.Size;
			Location = new Point(0, 0);

			// Make text smaller for preview window
			_fontSize = Size.Height / FontScaleFactor;

			_isPreviewMode = true;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Cursor.Hide();
			TopMost = true;

			moveTimer_Tick(null, null);
			moveTimer.Interval = 1000;
			moveTimer.Tick += moveTimer_Tick;
			moveTimer.Start();
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			// Some test times
			if (Debugger.IsAttached)
			{
				//SystemTime.NowForTesting = new DateTime(2020, 6, 7, 1, 23, 45);
				//SystemTime.NowForTesting = new DateTime(2020, 6, 7, 23, 45, 57);
			}
        }

		private void moveTimer_Tick(object sender, EventArgs e)
		{
			var now = SystemTime.Now;

			var minute = now.Minute;
			if (_lastMinute != minute)
			{
				// Update every minute
				_lastMinute = minute;
				DrawIt();
			}
			if (_showSeconds)
			{
				DrawIt();
			}

			// Move text to new location
			//			if (now.Second % 5 == 0)
			//			{
			//		        textLabel.Left = rand.Next(Math.Max(1, Bounds.Width - textLabel.Width));
			//		        textLabel.Top = rand.Next(Math.Max(1, Bounds.Height - textLabel.Height));
			//	        }
		}

        private PrivateFontCollection InitFonts()
        {
			// We don't add both fonts at the same time because I can only get the private font collection
			// to return the first one we add. If the first one is the non-bold one and we ask for a bold one
			// then it seems to have a go at generating bold rather than using the one we gave it.
			// The system font collection does not seem to have this problem.

			var pfc = new PrivateFontCollection();
			if (_screenSetting.DisplayType == DisplayType.CurrentTime)
            {
                AddFont(pfc, Properties.Resources.HelveticaLTStd_BoldCond);
            }
            else
            {
                AddFont(pfc, Properties.Resources.HelveticaLTStd_Cond);
            }
			return pfc;
        }

        private static void AddFont(PrivateFontCollection pfc, byte[] fontResource)
        {
            IntPtr ptr = Marshal.AllocCoTaskMem(fontResource.Length);  // create an unsafe memory block for the font data
            Marshal.Copy(fontResource, 0, ptr, fontResource.Length);  // copy the bytes to the unsafe memory block
			pfc.AddMemoryFont(ptr, fontResource.Length);    // pass the font to the font collection
            Marshal.FreeCoTaskMem(ptr);
		}

		private void DrawIt()
		{
			try
			{
				Gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
				Gfx.SmoothingMode = SmoothingMode.HighQuality;

				if (_isPreviewMode || _screenSetting.DisplayType == DisplayType.CurrentTime)
                {
					DrawCurrentTime();
				}
				else if (_screenSetting.DisplayType == DisplayType.WorldTime)
				{
					DrawCities();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void DrawCurrentTime()
		{
			var height = PrimaryFont.Height * 10/10;
			var width = !_showSeconds ? Convert.ToInt32(2.05*height) : Convert.ToInt32(3.1 * height);

			var x = (Width - width)/2;
			var y = (Height - height)/2;

            if (!_display24HourTime)
            {
				var pm = SystemTime.Now.Hour >= 12;
			    DrawIt(x, y, height, SystemTime.Now.ToString("%h"), pm ? null : "AM", pm ? "PM" : null); // The % avoids a FormatException https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx#UsingSingleSpecifiers
            }
            else
            {
                DrawIt(x, y, height, SystemTime.Now.ToString("HH"));
			}

			x += height + (height/20);
			DrawIt(x, y, height, SystemTime.Now.ToString("mm"));

			if (_showSeconds)
			{
				x += height + (height/20);
				DrawIt(x, y, height, SystemTime.Now.ToString("ss"));
			}	
		}

		private void DrawIt(int x, int y, int size, string s, string topString = null, string bottomString = null)
		{
			// Draw the background
			var diff = size/10;
			var textRect = new Rectangle(x - diff, y + diff/2, size + diff*2, size);

			var radius = size/20;
			var diameter = radius*2;
			Gfx.FillEllipse(_backFillTop, x, y, diameter, diameter); // top left
			Gfx.FillEllipse(_backFillTop, x + size - diameter, y, diameter, diameter); // top right
			Gfx.FillEllipse(_backFillBottom, x, y + size - diameter, diameter, diameter); // bottom left
			Gfx.FillEllipse(_backFillBottom, x + size - diameter, y + size - diameter, diameter, diameter); //bottom right

			Gfx.FillRectangle(_backFillTop, x + radius, y, size - diameter, diameter);
			Gfx.FillRectangle(_backFillBottom, x + radius, y + size - diameter, size - diameter, diameter);

			var linGrBrush = new LinearGradientBrush(
				new Point(10, y + radius),
				new Point(10, y + size - radius),
				BackColorTop,
				BackColorBottom);
			Gfx.FillRectangle(linGrBrush, x, y + radius, size, size - diameter);
			linGrBrush.Dispose();

//			if (s.Length == 1)
//			{
//				s = "\u2002" + s; // Add an EN SPACE which is 1/2 em
//			}

			// Draw the text
			var stringFormat = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
			Gfx.DrawString(s, PrimaryFont, _fontBrush, textRect, stringFormat);

			if (topString != null)
			{
				//Gfx.DrawString(bottomString, SmallFont, FontBrush, textRect.X, textRect.Bottom - SmallFont.Height);
				Gfx.DrawString(topString, PrimarySmallFont, _fontBrush, x + diameter, y + diameter);
			}
			if (bottomString != null)
			{
				Gfx.DrawString(bottomString, PrimarySmallFont, _fontBrush, x + diameter, y + size - diameter - PrimarySmallFont.Height);
			}

			// Horizontal dividing line
			if (!_isPreviewMode)
			{
				var penY = y + (size/2) - (SplitWidth/2);
				Gfx.DrawLine(_splitPen, x, penY, x + size, penY);
			}
			else
			{
				Gfx.DrawLine(Pens.Black, x, y + (size / 2), x + size, y + (size / 2));
			}
		}
		
		private void DrawCities()
		{
			const int dstIndicatorLength = 1;
			const int dayIndicatorLength = 3;
			const int maxBoxHeight = 160;
			
			var maxNameLengthInChars = Cities.Max(c => c.DisplayName.Length);
			
			var maxRowLengthInChars = _cityDisplayDstIndicatorStyle == DstIndicatorStyle.SmallDst 
				? maxNameLengthInChars + 2 + TimeLengthInChars + 1 + dstIndicatorLength 
				: maxNameLengthInChars + 2 + TimeLengthInChars + 1 + dayIndicatorLength;

			var maxWidth = Width - 40; // leave some margin
			var maxHeight = Height - 40;
			
			var roughBoxWidth = maxWidth / maxRowLengthInChars; // the rough max width of boxes
			var roughBoxHeight = maxHeight / Cities.Count;
			var boxHeight = Math.Min(roughBoxHeight, roughBoxWidth.PercentInv(BoxWidthPercentage));
			boxHeight -= boxHeight.Percent(VerticalGapBetweenBoxesPercent);
			boxHeight = Math.Min(boxHeight, maxBoxHeight);
				
			if (_cityFont == null)
			{
				_cityFont = new Font(FontFamily, boxHeight.Percent(80), FontStyle.Regular, GraphicsUnit.Pixel);
				_smallCityFont = new Font(FontFamily, boxHeight.Percent(25), FontStyle.Regular, GraphicsUnit.Pixel);
			}

			var verticalGap = boxHeight.Percent(VerticalGapBetweenBoxesPercent);

			//var heightForAllRows = (boxHeight + verticalGap) * cities.Count - verticalGap;
			var heightForAllRows = CalcSize(Cities.Count,boxHeight, verticalGap);
			var y = (Height - heightForAllRows) / 2;
			if (y < 20) 
				y = 20;
			var startingX = (Width - maxRowLengthInChars * boxHeight.Percent(BoxWidthPercentage)).Percent(50);

			var boxSize = new Size(boxHeight.Percent(BoxWidthPercentage), boxHeight);
			var horizontalGap = boxSize.Height.Percent(HorizontalGapBetweenBoxesPercent);
			
			foreach (var city in Cities.OrderBy(c => c.CurrentTime))
			{
				city.RefreshTime(SystemTime.Now);
				var s = city.DisplayName.PadRight(maxNameLengthInChars + 2) + FormatTime(city, _cityDisplayDstIndicatorStyle == DstIndicatorStyle.DstAsAsterisk);
				DrawString(startingX, y, boxSize, horizontalGap, s, city, _cityDisplayDstIndicatorStyle);
				y += boxHeight + verticalGap;
			}
		}

		private int CalcSize(int itemCount, int itemSize, int gapSize)
		{
			return (itemCount * (itemSize + gapSize)) - gapSize;
		}

		private string FormatTime(City city, bool appendAsteriskIfDst)
		{
			var suffix = appendAsteriskIfDst && city.IsDaylightSavingTime ? "*" : " ";
			var result = $"{city.CurrentTime:h:mm tt}{suffix}";
			return $"{result, TimeLengthInChars}";  // right aligned in 9 chars
		}

		private List<City> GetCities()
		{
			var result = new List<City>();
			
			result.Add(new City("Pacific Standard Time", "Los Angeles"));
			
			//return result;  // uncomment to testing one item
			
			result.Add(new City("Eastern Standard Time", "New York (EST)"));
			result.Add(new City("E. Australia Standard Time", "Brisbane"));
			result.Add(new City("New Zealand Standard Time", "Wellington"));
			result.Add(new City("SE Asia Standard Time", "Hanoi"));
			//result.Add(new City("AUS Eastern Standard Time", "Melbourne"));
			result.Add(new City("UTC", "UTC"));
			//result.Add(new City("W. Australia Standard Time", "Perth"));
			//result.Add(new City("Pakistan Standard Time", "Islamabad"));
			//result.Add(new City("Israel Standard Time", "Jerusalem"));
			result.Add(new City("GMT Standard Time", "London"));
			result.Add(new City("W. Europe Standard Time", "Stockholm"));
			result.Add(new City("Romance Standard Time", "Paris"));
			result.Add(new City("Hawaiian Standard Time", "Hawaii"));
			
			return result;
		}

		private void DrawString(int x, int y, Size boxSize, int horizontalGap, string s, City city, DstIndicatorStyle dstIndicatorStyle)
		{
			var boxRectangle = new Rectangle(new Point(x, y), boxSize);
			foreach (var c in s.ToUpperInvariant())
			{
				DrawCharInBox(boxRectangle, c);
				boxRectangle.X = boxRectangle.Right + horizontalGap;
			}

			if (dstIndicatorStyle == DstIndicatorStyle.SmallDst)
			{
				if (city.IsDaylightSavingTime || city.DaysDifference != 0)
				{
					DrawSmallStringsInBox(boxRectangle, 
						city.IsDaylightSavingTime ? "DST" : null,
						city.DaysDifference != 0 ? $"{city.DaysDifference:+#;-#}d" : null);
				}
				else
				{
					DrawCharInBox(boxRectangle, ' ');
				}
			}
			else
			{
				DrawStringInBoxes(boxRectangle, horizontalGap,
					city.DaysDifference != 0 
						? " " + city.CurrentTime.ToString("ddd") 
						: "    ");
			}
		}

		private int DrawStringInBoxes(Rectangle boxRectangle, int horizontalGap, string s)
		{
			foreach (var c in s.ToUpperInvariant())
			{
				DrawCharInBox(boxRectangle, c);
				boxRectangle.X = boxRectangle.Right + horizontalGap;
			}
			return boxRectangle.X;
		}

		private void DrawCharInBox(Rectangle boxRectangle, char theChar)
		{
			DrawBox(boxRectangle);
			DrawString(theChar.ToString(), _cityFont, boxRectangle);
			DrawSplitter(boxRectangle);
		}
		
		private void DrawSmallStringsInBox(Rectangle boxRectangle, string top, string bottom)
		{
			DrawBox(boxRectangle, top, bottom);
			DrawSplitter(boxRectangle);
		}

		private void DrawSplitter(Rectangle box)
		{
			var penY = box.Y + (box.Height/2) - (CityBoxSplitWidth/2);
			Gfx.DrawLine(_smallSplitPen, box.Left, penY, box.Right + 1, penY);
		}
		
		private void DrawBox(Rectangle box)
		{
			// Alternative, simple way to draw box
			// Gfx.FillRectangle(backFillTop, box.X, box.Y, box.Width, box.Height / 2);
			// Gfx.FillRectangle(backFillBottom, box.X, box.Y + (box.Height / 2), box.Width, box.Height / 2);

			var radius = box.Height / 20;
			
			var path = RoundedRectangle.Create(box, radius);
			Gfx.DrawPath(_cityBoxPen, path);
			Gfx.FillPath(_backFillTop, path);
		}

		private void DrawBox(Rectangle box, string topString, string bottomString)
		{
			var radius = box.Height / 20;
			var halfRectangle = new Rectangle(box.X, box.Y,box.Width, box.Height / 2);

			DrawHalfBox(halfRectangle, radius,RectangleCorners.TopLeft | RectangleCorners.TopRight, topString);
			
			// Move the rect down
			halfRectangle.Y = halfRectangle.Y + halfRectangle.Height + 1;
			
			DrawHalfBox(halfRectangle, radius,RectangleCorners.BottomLeft | RectangleCorners.BottomRight, bottomString);
		}

		private void DrawHalfBox(Rectangle halfRectangle, int radius, RectangleCorners corners, string s)
		{
			var path = RoundedRectangle.Create(halfRectangle, radius, corners);
			Gfx.DrawPath(_cityBoxPen, path);
			Gfx.FillPath(_backFillTop, path);
			if (!String.IsNullOrEmpty(s))
			{
				DrawString(s, _smallCityFont, halfRectangle);
			}
		}

		private void DrawString(string s, Font font, Rectangle box)
		{
			var stringFormat = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
			Gfx.DrawString(s, font, _fontBrush, box, stringFormat);
		}

		private void MainForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (!_isPreviewMode)
			{
				if (!_mouseLocation.IsEmpty)
				{
					// Terminate if mouse is moved a significant distance
					if (Math.Abs(_mouseLocation.X - e.X) > 5 ||
						Math.Abs(_mouseLocation.Y - e.Y) > 5)
						Application.Exit();
				}

				// Update current mouse location
				_mouseLocation = e.Location;
			}
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!_isPreviewMode)
			{
				Application.Exit();
			}
		}

		private void MainForm_MouseClick(object sender, MouseEventArgs e)
		{
			if (!_isPreviewMode)
			{
				Application.Exit();
			}
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			DrawIt();
		}

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisposeFontResources();
        }

        private void DisposeFontResources()
        {
			if (_fontFamily != null)
            {
                _fontFamily.Dispose();
                _fontFamily = null;
            }
            if (_pfc != null)
            {
                _pfc.Dispose();
                _pfc = null;
            }
		}
    }
}
