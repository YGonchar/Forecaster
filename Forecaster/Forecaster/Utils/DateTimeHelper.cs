using System;

namespace Forecaster.Utils
{
    public class DateTimeHelper
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(int unixTime)
        {
            var timeSpan = TimeSpan.FromSeconds(unixTime);
            return _epoch.Add(timeSpan).ToLocalTime();
        }
    }
}