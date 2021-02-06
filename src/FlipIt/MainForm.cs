/* Originally based on project by Frank McCown in 2010 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        // We store the mouse location so that we can ignore the mouse move event that
		// automatically occurs when the form is first shown
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
                        _timeScreen = _screenSetting.Locations.Count > 0 
                            ? new WorldTimesScreen(_screenSetting.Locations, this, _settings.Display24HrTime, _settings.ShowDstIndicator) 
                            : new WorldTimesScreen(GetDefaultLocations(), this, _settings.Display24HrTime, _settings.ShowDstIndicator);
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
                LogError(e);
            }
		}

        private static void LogError(Exception e)
        {
            var settingsFolder =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlipIt");
            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
            }

            File.AppendAllText(Path.Combine(settingsFolder, "Errors.txt"),
                $"{DateTime.Now} - {e}{Environment.NewLine}");
        }

        private List<Location> GetDefaultLocations()
		{
			var result = new List<Location>();
			
			result.Add(new Location("Pacific Standard Time", "Los Angeles"));
			result.Add(new Location("Eastern Standard Time", "New York (EST)"));
			result.Add(new Location("E. Australia Standard Time", "Brisbane"));
			result.Add(new Location("New Zealand Standard Time", "Wellington"));
			// result.Add(new Location("SE Asia Standard Time", "Hanoi"));
			result.Add(new Location("AUS Eastern Standard Time", "Melbourne"));
			result.Add(new Location("UTC", "UTC"));
			//result.Add(new Location("W. Australia Standard Time", "Perth"));
			//result.Add(new Location("Pakistan Standard Time", "Islamabad"));
			//result.Add(new Location("Israel Standard Time", "Jerusalem"));
			result.Add(new Location("GMT Standard Time", "London"));
			//result.Add(new Location("W. Europe Standard Time", "Stockholm"));
			result.Add(new Location("Romance Standard Time", "Paris"));
			result.Add(new Location("Hawaiian Standard Time", "Hawaii"));
			
			return result;
		}
		
		private void MainForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (!_isPreviewMode)
			{
				if (!_mouseLocation.IsEmpty)
				{
					if (_mouseLocation != e.Location)
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
            if (_timeScreen != null)
            {
                _timeScreen.DisposeResources();
            }
        }
    }
}
