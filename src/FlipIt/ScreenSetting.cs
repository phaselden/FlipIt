using System.Collections.Generic;

namespace ScreenSaver
{
    public class ScreenSetting
    {
        public ScreenSetting(int screenNumber, string deviceName, int width, int height)
        {
            ScreenNumber = screenNumber;
            DeviceName = deviceName;
            Width = width;
            Height = height;
        }

        public int ScreenNumber { get; }
        public string DeviceName { get;  }
        public int Width { get; }
        public int Height { get; }

        public DisplayType DisplayType { get; set; }

        public string ShortDescription => $"Screen {ScreenNumber}";
        public string Description => $"{ShortDescription} - {Width} x {Height}";

        public List<Location> Locations { get; set; } = new List<Location>();
    }
}