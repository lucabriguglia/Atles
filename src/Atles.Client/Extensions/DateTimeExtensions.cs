using System;

namespace Atles.Client.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUtcToLocalTime(this DateTime utcDate)
        {
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            var localTime = utcDate.ToLocalTime();
            return localTime;
        }

        public static string ToAppLocalDateAndTime(this DateTime timeStamp)
        {
            var localDate = timeStamp.FromUtcToLocalTime();
            var forumDateAndTime = $"{localDate.ToShortDateString()} {localDate.ToShortTimeString()}";
            return forumDateAndTime;
        }

        public static string ToAppLocalDate(this DateTime timeStamp)
        {
            var localDate = timeStamp.FromUtcToLocalTime();
            var forumDate = localDate.ToShortDateString();
            return forumDate;
        }
    }
}
