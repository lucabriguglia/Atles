using System;

namespace Atlas.Client.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUtcToLocalTime(this DateTime utcDate)
        {
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            var localTime = utcDate.ToLocalTime();
            return localTime;
        }

        public static string ToForumLocalDateAndTime(this DateTime timeStamp)
        {
            var localDate = timeStamp.FromUtcToLocalTime();
            var forumDateAndTime = $"{localDate.ToShortDateString()} {localDate.ToShortTimeString()}";
            return forumDateAndTime;
        }
    }
}
