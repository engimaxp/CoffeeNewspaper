using System;
using System.Text;

namespace CN_Core.Utilities
{
    public static class TimeSpanHelper
    {
        public static string GetTimeSpanLeftInfo(this TimeSpan timeSpan)
        {
            StringBuilder result = new StringBuilder();

            //these two var is used to determine how many D\H\M\S is show
            //only show max-count units of time.like 1D2H
            int count = 0;
            int max = 2;

            if (timeSpan.Days >= 1)
            {
                result.AppendFormat("{0}D", timeSpan.Days);
                count++;
            }

            if (timeSpan.Hours >= 1 && count < max)
            {
                result.AppendFormat("{0}H", timeSpan.Hours);
                count++;
            }

            if (timeSpan.Minutes >= 1 && count < max)
            {
                result.AppendFormat("{0}M", timeSpan.Minutes);
                count++;
            }

            if (timeSpan.Seconds >= 1 && count < max)
            {
                result.AppendFormat("{0}S", timeSpan.Seconds);
            }

            //if the time left is less than 1 sec than return right now
            if (result.Length == 0)
            {
                result.Append("Right Now");
            }
            else
            {
                //add a left sign to the end of the timetip
                result.Append(" Left");
            }
            return result.ToString();
        }
    }
}