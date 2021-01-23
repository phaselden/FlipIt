using System;

namespace ScreenSaver
{
    internal static class Int32Extensions
    {
        internal static int Percent(this int value, int percent)
        {
            return value * percent / 100;
        }

        internal static int Percent(this int value, double percent)
        {
            return Convert.ToInt32(value * percent / 100);
        }

        internal static int PercentInv(this int value, int percent)
        {
            return value * 100 / percent;
        }
    }
}