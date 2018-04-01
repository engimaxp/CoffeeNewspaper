using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public static class TimeSlicesStatistics
    {
        public static TimeSpan GetTimeSliceDuration(this CNTimeSlice timeSlice,DateTime referenceTime)
        {
            if(timeSlice==null)return TimeSpan.Zero;
            var endtime = timeSlice.EndDateTime ?? referenceTime;
            if(endtime<=timeSlice.StartDateTime)return TimeSpan.Zero;
            return endtime - timeSlice.StartDateTime;
        }

        /// <summary>
        /// How many hours have used
        /// </summary>
        /// <param name="timeSlices"></param>
        /// <returns></returns>
        public static double GetTotalHours(this IEnumerable<CNTimeSlice> timeSlices)
        {
            var referenceTime = DateTime.Now;
            return timeSlices.Sum(timeSlice => timeSlice.GetTimeSliceDuration(referenceTime).TotalHours);
        }
        /// <summary>
        /// How many days have used
        /// </summary>
        /// <param name="timeSlices"></param>
        /// <returns></returns>
        public static double GetTotalDays(this IEnumerable<CNTimeSlice> timeSlices)
        {
            var referenceTime = DateTime.Now;
            return timeSlices.Sum(timeSlice => timeSlice.GetTimeSliceDuration(referenceTime).TotalDays);
        }
        /// <summary>
        /// How many days have passed
        /// </summary>
        /// <param name="timeSlices"></param>
        /// <returns></returns>
        public static int GetWorkDays(this IEnumerable<CNTimeSlice> timeSlices)
        {
            int result = 0;
            HashSet<CNTimeSlice> days = new HashSet<CNTimeSlice>();
            foreach (var timeSlice in timeSlices)
            {
                var dayDuration = timeSlice.GetDayDuration();
                if (dayDuration == null) continue;
                //small or equal than current
                if (days.Any(x => x.IsContaining(dayDuration)))
                {
                    continue;
                }
                //big than current
                var tobeReplaced = days.FirstOrDefault(x => dayDuration.IsContaining(x));
                if (tobeReplaced!=null)
                {
                    days.Remove(tobeReplaced);
                    days.Add(dayDuration);
                }
                else
                {
                    //none of current.
                    days.Add(dayDuration);
                }
            }

            foreach (var cnTimeSlice in days)
            {
                result += cnTimeSlice.GetWorkDays();
            }
            return result;
        }
    }
}
