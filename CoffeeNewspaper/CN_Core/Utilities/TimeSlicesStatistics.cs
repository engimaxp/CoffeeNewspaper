using System;
using System.Collections.Generic;
using System.Linq;

namespace CN_Core
{
    /// <summary>
    ///     Helper class to Get TimeSlices Durations
    /// </summary>
    public static class TimeSlicesStatistics
    {
        /// <summary>
        ///     Calculate a single timeslice's duration
        /// </summary>
        /// <param name="timeSlice">the <see cref="CNTimeSlice" /> to calculate</param>
        /// <param name="referenceTime">
        ///     if the endtime is null,than use referenceTIme as the endTime
        ///     most cases this value is now
        /// </param>
        /// <returns>the deviations TimeSpan of the StartDateTime and EndDateTime</returns>
        public static TimeSpan GetTimeSliceDuration(this CNTimeSlice timeSlice, DateTime referenceTime)
        {
            if (timeSlice == null) return TimeSpan.Zero;
            var endtime = timeSlice.EndDateTime ?? referenceTime;
            if (endtime <= timeSlice.StartDateTime) return TimeSpan.Zero;
            return endtime - timeSlice.StartDateTime;
        }

        /// <summary>
        ///     Calculate mutiple timeslices's hour Duration
        /// </summary>
        /// <param name="timeSlices">the timeslicesList to calculate</param>
        /// <returns>double value of the total hours</returns>
        public static double GetTotalHours(this IEnumerable<CNTimeSlice> timeSlices)
        {
            var referenceTime = DateTime.Now;
            return timeSlices.Sum(timeSlice => timeSlice.GetTimeSliceDuration(referenceTime).TotalHours);
        }

        /// <summary>
        ///     Calculate mutiple timeslices's day Duration
        /// </summary>
        /// <param name="timeSlices">the timeslicesList to calculate</param>
        /// <returns>double value of the total days</returns>
        public static double GetTotalDays(this IEnumerable<CNTimeSlice> timeSlices)
        {
            var referenceTime = DateTime.Now;
            return timeSlices.Sum(timeSlice => timeSlice.GetTimeSliceDuration(referenceTime).TotalDays);
        }

        /// <summary>
        ///     Calculate workday count the timeslices has encontered
        /// </summary>
        /// <param name="timeSlices">the timeslicesList to calculate</param>
        /// <returns>double value of the total encontered days</returns>
        public static int GetWorkDays(this IEnumerable<CNTimeSlice> timeSlices)
        {
            //store the maximum day periods
            var days = new HashSet<CNTimeSlice>(CNTimeSlice.StartDateTimeEndDateTimeComparer);
            foreach (var timeSlice in timeSlices)
            {
                //Get encounted maximum day period
                var dayDuration = timeSlice?.GetDayDuration();
                if (dayDuration == null) continue;
                //if current day period is smaller than one of the existing day period then jump to next
                if (days.Any(x => x.IsContaining(dayDuration))) continue;
                //if bigger than existing day period then replace it
                var tobeReplaced = days.FirstOrDefault(x => dayDuration.IsContaining(x));
                if (tobeReplaced != null)
                {
                    days.Remove(tobeReplaced);
                    days.Add(dayDuration);
                }
                //if current day period has no intersections with existing day period list then add it
                else
                {
                    days.Add(dayDuration);
                }
            }

            return days.ToList().Sum(x => x.GetWorkDays());
        }
    }
}