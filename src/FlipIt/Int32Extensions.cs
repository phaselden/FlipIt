namespace ScreenSaver
{
    static class Int32Extensions
    {
        internal static int Percent(this int value, int percent)
        {
            return value * percent / 100;
        }
		
        internal static int PercentInv(this int value, int percent)
        {
            return value * 100 / percent;
        }
    }
}