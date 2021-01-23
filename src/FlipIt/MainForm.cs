/* Originally based on project by Frank McCown in 2010 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        private TimeScreen _timeScreen;

		private readonly FlipItSettings _settings;
        private readonly ScreenSetting _screenSetting;
		private Point _mouseLocation;
		private readonly bool _isPreviewMode;
		private readonly bool _showSeconds = false;
		private int _lastMinute = -1;

        public MainForm()
		{
			InitializeComponent();
		}

		public MainForm(Rectangle bounds, FlipItSettings settings, ScreenSetting screenSetting)
		{
            _settings = settings;
            _screenSetting = screenSetting;
			InitializeComponent();
			Bounds = bounds;
        }

		public MainForm(IntPtr previewWndHandle, FlipItSettings settings, ScreenSetting screenSetting)
		{
			_settings = settings;
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
				PaintTime();
			}
			if (_showSeconds)
			{
				PaintTime();
			}

			// Move text to new location
			//			if (now.Second % 5 == 0)
			//			{
			//		        textLabel.Left = rand.Next(Math.Max(1, Bounds.Width - textLabel.Width));
			//		        textLabel.Top = rand.Next(Math.Max(1, Bounds.Height - textLabel.Height));
			//	        }
		}

        private void PaintTime()
		{
			try
			{
				if (_timeScreen == null)
                {
                    if (_isPreviewMode || _screenSetting.DisplayType == DisplayType.CurrentTime)
                    {
                        _timeScreen = new CurrentTimeScreen(this, _settings.Display24HrTime, _isPreviewMode, _settings.Scale);
					}
                    else if (_screenSetting.DisplayType == DisplayType.WorldTime)
                    {
                        var cities = GetCities();
                        _timeScreen = new WorldTimesScreen(cities, this);
					}
                    else
                    {
                        throw new NotImplementedException("Unhandled state: " + _screenSetting.DisplayType);
                    }
                }
                _timeScreen.Draw();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
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
			PaintTime();
		}

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _timeScreen.DisposeResources();
        }
    }
}
