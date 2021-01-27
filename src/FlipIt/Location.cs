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

        public string DisplayName { get; set; }
        public string TimeZoneID { get; }
        public DateTime CurrentTime { get; private set; }
        public bool IsDaylightSavingTime { get; private set; }
        public int DaysDifference { get; private set; }

        /// <summary>
        /// Is true if the underlying TimeZoneID cannot be found. This could potentially happen if timezones change
        /// and since we are using a static generated TimeZoneCities file, it might be out of date.
        /// </summary>
        public bool HasError { get; private set; }

        public void RefreshTime(DateTime now)
        {
            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
                CurrentTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, timeZoneInfo);
                IsDaylightSavingTime = timeZoneInfo.IsDaylightSavingTime(CurrentTime);
                DaysDifference = (CurrentTime.Date - now.Date).Days;
                HasError = false;
            }
            catch (TimeZoneNotFoundException)
            {
                HasError = true;
            }
        }
    }
}