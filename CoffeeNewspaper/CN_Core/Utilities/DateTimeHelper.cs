using System;

namespace CN_Core.Utilities
{
    public static class DateTimeHelper
    {
        public static string EarlyTimeFormat(this DateTime time,DateTime? refrenceTime = null)
        {
            var now = refrenceTime??DateTime.Now;
            var diff = now - time;
            var diffDate = now.Date - time.Date;
            if (diff.TotalHours < 7)
            {
                return MinuteAgo(diff);
            }
            else if (now.AddMonths(-1) <= time)
            {
                return EarlyToday(time, now, diffDate);
            }
            else if (now.AddMonths(-1) > time && now.AddMonths(-2) <= time) return "1 Month Ago";
            else if (now.AddMonths(-2) > time && now.AddYears(-1) <= time)
            {
                return MonthsAgo(time, diff, now);
            }
            else if (now.AddYears(-1) >= time && now.AddYears(-2) <= time) return "1 Year Ago";
            else if (now.AddYears(-2) > time)
            {
                return YearsAgo(time, diff, now);
            }

            return "Error";
        }

        private static string YearsAgo(DateTime time, TimeSpan diff, DateTime now)
        {
            var n = diff.Days / 365;
            if (now.AddYears(-n) <= time && now.AddYears(-n + 1) > time)
                return $"{n} Years Ago";
            else if (now.AddYears(-n - 1) <= time && now.AddYears(-n) > time)
                return $"{n + 1} Years Ago";
            else if (now.AddYears(-n + 1) <= time && now.AddYears(-n + 2) > time)
                return $"{n - 1} Years Ago";
            else return "Error Years";
        }

        private static string MonthsAgo(DateTime time, TimeSpan diff, DateTime now)
        {
            var n = diff.Days / 30;
            if (now.AddMonths(-n) <= time && now.AddMonths(-n + 1) > time)
                return $"{n} Months Ago";
            else if (now.AddMonths(-n - 1) <= time && now.AddMonths(-n) > time)
                return $"{n + 1} Months Ago";
            else if (now.AddMonths(-n + 1) <= time && now.AddMonths(-n + 2) > time)
                return $"{n - 1} Months Ago";
            else return "Error Months";
        }

        private static string MinuteAgo(TimeSpan diff)
        {
            if (diff.TotalSeconds < 60) return "Just Now";
            else if (diff.TotalSeconds >= 60 && diff.TotalSeconds < 120) return "1 Minute Ago";
            else if (diff.TotalSeconds >= 120 && diff.TotalMinutes < 60) return $"{diff.Minutes} Minutes Ago";
            else if (diff.TotalSeconds >= 3600 && diff.TotalSeconds < 3600 * 2) return "1 Hour Ago";
            else return $"{diff.Hours} Hours Ago";
        }

        private static string EarlyToday(DateTime time, DateTime now, TimeSpan diffDate)
        {
            if (now.Date == time.Date) return "Early Today";
            else if (now.Date == time.Date.AddDays(1)) return "Yesterday";
            else if (diffDate.TotalDays >= 2 && diffDate.TotalDays < 7) return $"{diffDate.Days} Days Ago";
            else if (diffDate.TotalDays >= 7 && diffDate.TotalDays < 7 * 2) return "1 Week Ago";
            else return $"{(int) diffDate.TotalDays / 7} Weeks Ago";
        }
    }
}