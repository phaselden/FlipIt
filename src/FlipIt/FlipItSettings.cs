using System.Collections.Generic;
using System.Linq;

namespace ScreenSaver
{
    public class FlipItSettings
    {
        public bool Display24HrTime { get; set; }
        public int Scale { get; set; } = 70;

        public List<ScreenSetting> ScreenSettings { get; set; } = new List<ScreenSetting>();

        public ScreenSetting GetScreen(string screenDeviceName)
        {
            return ScreenSettings.SingleOrDefault(s => s.DeviceName == screenDeviceName);
        }
    }
}