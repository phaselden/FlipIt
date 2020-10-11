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
		private const int FontScaleFactor = 3;
		
		private readonly bool isPrimaryScreen;
		private Point mouseLocation;
		private readonly bool previewMode = false;
		private readonly bool showSeconds = false;
		private int lastMinute = -1;
		private readonly int fontSize = 350;
		private Font primaryFont;
		private Font cityFont;
		private Font smallCityFont;
		private Font primarySmallFont;
		private Graphics graphics;

		// Alternative fonts:
		// * league-gothic from https://github.com/theleagueof/league-gothic
		// * http://tipotype.com/aileron/

		private const string familyName = "Oswald"; //"Texgyreheroscn";"Bebas"

		private Graphics Gfx => graphics ?? (graphics = CreateGraphics());

		private Font PrimaryFont => primaryFont ?? (primaryFont = new Font(familyName, fontSize, FontStyle.Bold));
		private Font PrimarySmallFont => primarySmallFont ?? (primarySmallFont = new Font(familyName, fontSize / 9, FontStyle.Bold));

		private static readonly Color backColorTop = Color.FromArgb(255, 15, 15, 15);
		private static readonly Color backColorBottom = Color.FromArgb(255, 10, 10, 10);

		private readonly Brush backFillTop = new SolidBrush(backColorTop);
		private readonly Brush backFillBottom = new SolidBrush(backColorBottom);
		private readonly Brush FontBrush = new SolidBrush(Color.FromArgb(255, 183, 183, 183));
		private readonly Pen SplitPen = new Pen(Color.Black, SplitWidth);
		private readonly Pen SmallSplitPen = new Pen(Color.Black, SplitWidth / 2);
		
		const int BoxWidthPercentage = 70;
		private const int HorizontalGapBetweenBoxesPercent = 5;
		private const int VerticalGapBetweenBoxesPercent = 10;

		private List<City> cities;
		private List<City> Cities => cities ?? (cities = GetCities());
		
		public MainForm()
		{
			InitializeComponent();
		}

		public MainForm(Rectangle bounds, bool isPrimaryScreen)
		{
			this.isPrimaryScreen = isPrimaryScreen;
			InitializeComponent();
			Bounds = bounds;
			fontSize = bounds.Height / FontScaleFactor;
		}

		public MainForm(IntPtr previewWndHandle)
		{
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
			fontSize = Size.Height / FontScaleFactor;

			previewMode = true;
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
			if (lastMinute != minute)
			{
				// Update every minute
				lastMinute = minute;
				DrawIt();
			}
			if (showSeconds)
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

		private void DrawIt()
		{
			Gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

			if (isPrimaryScreen || previewMode)
			{
				DrawCurrentTime();
			}
			else
			{
				DrawCities();
			}
		}

		private void DrawCurrentTime()
		{
			var height = PrimaryFont.Height*10/11;
			var width = !showSeconds ? Convert.ToInt32(2.05*height) : Convert.ToInt32(3.1 * height);

			var x = (Width - width)/2;
			var y = (Height - height)/2;

			var pm = SystemTime.Now.Hour >= 12;
			DrawIt(x, y, height, SystemTime.Now.ToString("%h"), pm ? null : "AM", pm ? "PM" : null); // The % avoids a FormatException https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx#UsingSingleSpecifiers

			x += height + (height/20);
			DrawIt(x, y, height, SystemTime.Now.ToString("mm"));

			if (showSeconds)
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
			Gfx.FillEllipse(backFillTop, x, y, diameter, diameter); // top left
			Gfx.FillEllipse(backFillTop, x + size - diameter, y, diameter, diameter); // top right
			Gfx.FillEllipse(backFillBottom, x, y + size - diameter, diameter, diameter); // bottom left
			Gfx.FillEllipse(backFillBottom, x + size - diameter, y + size - diameter, diameter, diameter); //bottom right

			Gfx.FillRectangle(backFillTop, x + radius, y, size - diameter, diameter);
			Gfx.FillRectangle(backFillBottom, x + radius, y + size - diameter, size - diameter, diameter);

			var linGrBrush = new LinearGradientBrush(
				new Point(10, y + radius),
				new Point(10, y + size - radius),
				backColorTop,
				backColorBottom);
			Gfx.FillRectangle(linGrBrush, x, y + radius, size, size - diameter);
			linGrBrush.Dispose();

//			if (s.Length == 1)
//			{
//				s = "\u2002" + s; // Add an EN SPACE which is 1/2 em
//			}

			// Draw the text
			var stringFormat = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
			Gfx.DrawString(s, PrimaryFont, FontBrush, textRect, stringFormat);

			if (topString != null)
			{
				//Gfx.DrawString(bottomString, SmallFont, FontBrush, textRect.X, textRect.Bottom - SmallFont.Height);
				Gfx.DrawString(topString, PrimarySmallFont, FontBrush, x + diameter, y + diameter);
			}
			if (bottomString != null)
			{
				Gfx.DrawString(bottomString, PrimarySmallFont, FontBrush, x + diameter, y + size - diameter - PrimarySmallFont.Height);
			}

			// Horizontal dividing line
			if (!previewMode)
			{
				var penY = y + (size/2) - (SplitWidth/2);
				Gfx.DrawLine(SplitPen, x, penY, x + size, penY);
			}
			else
			{
				Gfx.DrawLine(Pens.Black, x, y + (size / 2), x + size, y + (size / 2));
			}
		}
		
		private void DrawCities()
		{
			const int timeLengthInChars = 8;
			const int dstIndicatorLength = 1;
			const int maxBoxHeight = 160;
			
			var maxNameLengthInChars = Cities.Max(c => c.DisplayName.Length);
			var maxRowLengthInChars = maxNameLengthInChars + 2 + timeLengthInChars + 1 + dstIndicatorLength;

			var maxWidth = Width - 40; // leave some margin
			var maxHeight = Height - 40;
			
			var roughBoxWidth = maxWidth / maxRowLengthInChars; // the rough max width of boxes
			var roughBoxHeight = maxHeight / Cities.Count;
			var boxHeight = Math.Min(roughBoxHeight, roughBoxWidth.PercentInv(BoxWidthPercentage));
			boxHeight -= boxHeight.Percent(VerticalGapBetweenBoxesPercent);
			boxHeight = Math.Min(boxHeight, maxBoxHeight);
				
			if (cityFont == null)
			{
				cityFont = new Font(familyName, boxHeight.Percent(80), FontStyle.Regular, GraphicsUnit.Pixel);
				smallCityFont = new Font(familyName, boxHeight.Percent(25), FontStyle.Regular, GraphicsUnit.Pixel);
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
				var s = city.DisplayName.PadRight(maxNameLengthInChars + 2) + FormatTime(city.CurrentTime);
				DrawString(startingX, y, boxSize, horizontalGap, s, city);
				y += boxHeight + verticalGap;
			}
		}

		private int CalcSize(int itemCount, int itemSize, int gapSize)
		{
			return (itemCount * (itemSize + gapSize)) - gapSize;
		}

		private string FormatTime(DateTime time)
		{
			var result = time.ToString("h:mm tt") + " ";
			return $"{result,9}";
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

		private void DrawString(int x, int y, Size boxSize, int horizontalGap, string s, City city)
		{
			var boxRectangle = new Rectangle(new Point(x, y), boxSize);
			foreach (var c in s.ToUpperInvariant())
			{
				DrawCharInBox(boxRectangle, c);
				boxRectangle.X = boxRectangle.Right + horizontalGap;
			}

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
		
		private void DrawCharInBox(Rectangle boxRectangle, char theChar)
		{
			DrawBox(boxRectangle);
			DrawString(theChar.ToString(),cityFont, boxRectangle);
			DrawSplitter(boxRectangle);
		}
		
		private void DrawSmallStringsInBox(Rectangle boxRectangle, string top, string bottom)
		{
			DrawBox(boxRectangle, top, bottom);
			DrawSplitter(boxRectangle);
		}

		private void DrawSplitter(Rectangle box)
		{
			var penY = box.Y + (box.Height/2) - (SplitWidth/2/2); 
			Gfx.DrawLine(SmallSplitPen, box.Left, penY, box.Right, penY);
		}

		private void DrawBox(Rectangle box, string topString = null, string bottomString = null)
		{
			// Alternative, simple way to draw box
			// Gfx.FillRectangle(backFillTop, box.X, box.Y, box.Width, box.Height / 2);
			// Gfx.FillRectangle(backFillBottom, box.X, box.Y + (box.Height / 2), box.Width, box.Height / 2);
			
			var radius = box.Height / 20;
			var halfRectangle = new Rectangle(box.X, box.Y,box.Width, box.Height / 2);

			DrawHalfBox(halfRectangle, radius,RectangleCorners.TopLeft | RectangleCorners.TopRight, topString);
			
			// Move the rect down
			halfRectangle.Y = halfRectangle.Y + halfRectangle.Height + 1;
			
			DrawHalfBox(halfRectangle, radius,RectangleCorners.BottomLeft | RectangleCorners.BottomRight, bottomString);
		}

		private void DrawHalfBox(Rectangle halfRectangle, int radius, RectangleCorners corners,string s)
		{
			var path = RoundedRectangle.Create(halfRectangle, radius, corners);
			Gfx.FillPath(backFillTop, path);
			if (!String.IsNullOrEmpty(s))
			{
				DrawString(s, smallCityFont, halfRectangle);
			}
		}
		

		private void DrawString(string s, Font font, Rectangle box)
		{
			var stringFormat = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
			Gfx.DrawString(s, font, FontBrush, box, stringFormat);
		}

		private void MainForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (!previewMode)
			{
				if (!mouseLocation.IsEmpty)
				{
					// Terminate if mouse is moved a significant distance
					if (Math.Abs(mouseLocation.X - e.X) > 5 ||
						Math.Abs(mouseLocation.Y - e.Y) > 5)
						Application.Exit();
				}

				// Update current mouse location
				mouseLocation = e.Location;
			}
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!previewMode)
			{
				Application.Exit();
			}
		}

		private void MainForm_MouseClick(object sender, MouseEventArgs e)
		{
			if (!previewMode)
			{
				Application.Exit();
			}
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			DrawIt();
		}
	}
}
