using System;

namespace ScreenSaver
{
    public class Location
    {
        public Location(string timeZoneID, string displayName)
        {
            DisplayName = displayName;
            TimeZoneID = timeZoneID;
        }

        public string DisplayName { get; }
        public string TimeZoneID { get; }
        public DateTime CurrentTime { get; private set; }
        public bool IsDaylightSavingTime { get; private set; }
        public int DaysDifference { get; private set; }

        public void RefreshTime(DateTime now)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            CurrentTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, timeZoneInfo);
            IsDaylightSavingTime = timeZoneInfo.IsDaylightSavingTime(CurrentTime);
            DaysDifference = (CurrentTime.Date - now.Date).Days;
        }
    }
}