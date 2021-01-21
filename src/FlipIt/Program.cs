/* Originally based on project by Frank McCown in 2010 */

using System;
using System.IO;
using System.Windows.Forms;

namespace ScreenSaver
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            var settings = LoadSettings();

			if (args.Length > 0)
			{
				string firstArgument = args[0].ToLower().Trim();
				string secondArgument = null;

				// Handle cases where arguments are separated by colon.
				// Examples: /c:1234567 or /P:1234567
				if (firstArgument.Length > 2)
				{
					secondArgument = firstArgument.Substring(3).Trim();
					firstArgument = firstArgument.Substring(0, 2);
				}
				else if (args.Length > 1)
					secondArgument = args[1];

                if (firstArgument == "/c")           // Configuration mode
				{
					Application.Run(new SettingsForm(settings));
				}
				else if (firstArgument == "/p")      // Preview mode
				{
					if (secondArgument == null)
					{
						MessageBox.Show("Sorry, but the expected window handle was not provided.",
							"ScreenSaver", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}

					IntPtr previewWndHandle = new IntPtr(long.Parse(secondArgument));
					Application.Run(new MainForm(previewWndHandle, settings.Display24HrTime, settings.ScreenSettings[0]));
				}
				else if (firstArgument == "/s")      // Full-screen mode
				{
					ShowScreenSaver(settings);
					Application.Run();
				}
				else    // Undefined argument
				{
					MessageBox.Show("Sorry, but the command line argument \"" + firstArgument +
						"\" is not valid.", "FlipIt",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else    // No arguments - treat like /c
			{
				Application.Run(new SettingsForm(settings));
			}
		}

		/// <summary>
		/// Display the form on each of the computer's monitors.
		/// </summary>
		static void ShowScreenSaver(FlipItSettings settings)
        {
			foreach (var screen in Screen.AllScreens)
            {
                var cleanDeviceName = CleanDeviceName(screen.DeviceName);
                var screenSettings = settings.GetScreen(cleanDeviceName);
				var form = new MainForm(screen.Bounds, settings.Display24HrTime, screenSettings);
				form.Show();
			}
		}

        private static FlipItSettings LoadSettings()
        {
            var allScreens = Screen.AllScreens;
			IniFile iniFile = null;

			var settings = new FlipItSettings();

			var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlipIt");
			var iniFilePath = Path.Combine(settingsFolder, "Settings.ini");
            if (File.Exists(iniFilePath))
            {
                iniFile = new IniFile(iniFilePath);
                settings.Display24HrTime = iniFile.ReadBool("General", "Display24Hr", false);
            }
            else
            {
                settings.Display24HrTime = false;
            }

            var screenNum = 0;
			foreach (var screen in allScreens)
            {
                screenNum++;
                var cleanDeviceName = CleanDeviceName(screen.DeviceName);
		        var sectionName = $"Screen {cleanDeviceName}";
		    
                var screenSetting = new ScreenSetting(screenNum, cleanDeviceName, screen.Bounds.Width, screen.Bounds.Height);
                if (iniFile != null && iniFile.SectionExists(sectionName))
                {
                    screenSetting.DisplayType = (DisplayType) iniFile.ReadInt(sectionName, "DisplayType", (int) DisplayType.CurrentTime);
                }
                else
                {
                    screenSetting.DisplayType = DisplayType.CurrentTime;
                }
                settings.ScreenSettings.Add(screenSetting);
            }
			
			return settings;
        }

        private static string CleanDeviceName(string deviceName)
        {
            return deviceName.TrimStart(new[] {'\\', '.'});
        }
    }
}
