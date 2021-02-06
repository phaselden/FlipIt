using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScreenSaver
{
    public class FlipItSettings
    {
        // Make it creatable only via Load command
        private FlipItSettings()
        {
        }

        public bool Display24HrTime { get; set; }
        public bool ShowDstIndicator { get; set; }
        public int Scale { get; set; } = 70;

        public List<ScreenSetting> ScreenSettings { get; set; } = new List<ScreenSetting>();

        public ScreenSetting GetScreen(string screenDeviceName)
        {
            var cleanName = CleanScreenDeviceName(screenDeviceName);
            return ScreenSettings.SingleOrDefault(s => s.DeviceName == cleanName);
        }

        public static FlipItSettings Load(Screen[] allScreens)
        {
            IniFile iniFile = null;
            
            var settings = new FlipItSettings();
            var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlipIt");
            var iniFilePath = Path.Combine(settingsFolder, "Settings.ini");
            if (File.Exists(iniFilePath))
            {
                iniFile = new IniFile(iniFilePath);
                settings.Display24HrTime = iniFile.GetBool("General", "Display24Hr", false);
                settings.ShowDstIndicator = iniFile.GetBool("General", "ShowDstIndicator", true);
                settings.Scale = iniFile.GetInt("General", "Scale", 70);
            }
            else
            {
                settings.Display24HrTime = false;
            }

            var screenNum = 0;
            foreach (var screen in allScreens)
            {
                screenNum++;
                var cleanDeviceName = CleanScreenDeviceName(screen.DeviceName);
                var screenSectionName = $"Screen {cleanDeviceName}";

                var screenSetting = new ScreenSetting(screenNum, cleanDeviceName, screen.Bounds.Width, screen.Bounds.Height);
                if (iniFile != null)
                {
                    // if (iniFile.SectionExists(screenSectionName))
                    if (iniFile.SectionExists(screenSectionName))
                    {
                        screenSetting.DisplayType = (DisplayType)iniFile.GetInt(screenSectionName, "DisplayType", (int)DisplayType.CurrentTime);
                    }

                    var screenLocationsSectionName = $"Screen {cleanDeviceName} Locations";
                    if (iniFile.SectionExists(screenLocationsSectionName))
                    {
                        var locationNames = iniFile.GetKeys(screenLocationsSectionName);
                        foreach (var locationName in locationNames)
                        {
                            var timeZoneID = iniFile.GetString(screenLocationsSectionName, locationName);
                            if (!String.IsNullOrWhiteSpace(timeZoneID))
                            {
                                screenSetting.Locations.Add(new Location(timeZoneID, locationName));
                            }
                            else
                            {
                                iniFile.DeleteKey(screenLocationsSectionName, locationName);
                            }
                        }
                    }
                }
                else
                {
                    screenSetting.DisplayType = DisplayType.CurrentTime;
                }

                settings.ScreenSettings.Add(screenSetting);
            }
            return settings;
        }

        public void Save()
        {
            var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlipIt");
            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
            }
            var iniFilePath = Path.Combine(settingsFolder, "Settings.ini");
            if (!File.Exists(iniFilePath))
            {
                File.Create(iniFilePath).Dispose(); // create an empty file
            }

            var iniFile = new IniFile(iniFilePath);
            iniFile.SetBool("General", "Display24Hr", Display24HrTime);
            iniFile.SetBool("General", "ShowDstIndicator", ShowDstIndicator);
            iniFile.SetInt("General", "Scale", Scale);

            foreach (var screenSetting in ScreenSettings)
            {
                var screenSectionName = $"Screen {screenSetting.DeviceName}";
                iniFile.SetInt(screenSectionName, "DisplayType", (int)screenSetting.DisplayType);

                var screenLocationsSectionName = $"Screen {screenSetting.DeviceName} Locations";
                if (iniFile.SectionExists(screenLocationsSectionName))
                {
                    iniFile.DeleteSection(screenLocationsSectionName);
                }
                foreach (var location in screenSetting.Locations)
                {
                    iniFile.SetString(screenLocationsSectionName, location.DisplayName, location.TimeZoneID);
                }
            }
            iniFile.Save();
        }

        private static string CleanScreenDeviceName(string deviceName)
        {
            return deviceName.TrimStart('\\', '.');
        }
    }
}