using System;

namespace ScreenSaver
{
    internal static class StringExtensions
    {
        internal static bool HasSameText(this string value, string otherValue)
        {
            return String.Equals(value, otherValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}