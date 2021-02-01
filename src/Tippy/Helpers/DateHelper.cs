using System;
namespace Tippy.Helpers
{
    public static class DateHelper
    {
        public static DateTime TimestampToDate(string timestamp)
        { 
            DateTime date = new (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return date.AddSeconds(double.Parse(timestamp) / 1000).ToLocalTime();
        }

        public static DateTime HexTimestampToDate(string hex)
        {
            var timestamp = NumberHelper.HexToNumber(hex);
            DateTime date = new (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return date.AddSeconds(double.Parse(timestamp) / 1000).ToLocalTime();
        }
    }
}
