using System;

namespace ScreenSaver
{
    internal class City
    {
        public City(string timeZoneID, string displayName)
        {
            DisplayName = displayName;
            TimeZoneID = timeZoneID;
        }

        internal string DisplayName { get; }
        internal string TimeZoneID { get; }
        internal DateTime CurrentTime { get; private set; }
        internal bool IsDaylightSavingTime { get; private set; }
        internal int DaysDifference { get; private set; }

        internal void RefreshTime(DateTime now)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            CurrentTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, timeZoneInfo);
            IsDaylightSavingTime = timeZoneInfo.IsDaylightSavingTime(CurrentTime);
            DaysDifference = (CurrentTime.Date - now.Date).Days;
        }
    }
}