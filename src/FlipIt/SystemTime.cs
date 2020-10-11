using System;

namespace ScreenSaver
{
    public static class SystemTime
    {
        public static DateTime? NowForTesting { get; set; }

        public static DateTime Now => NowForTesting ?? DateTime.Now;
    }
}