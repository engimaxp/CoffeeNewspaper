using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNTimeSlice:IEquatable<CNTimeSlice>,ICloneable, IComparable<CNTimeSlice>
    {
        private sealed class StartDateTimeEndDateTimeEqualityComparer : IEqualityComparer<CNTimeSlice>
        {
            public bool Equals(CNTimeSlice x, CNTimeSlice y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.StartDateTime.Equals(y.StartDateTime) && x.EndDateTime.Equals(y.EndDateTime);
            }

            public int GetHashCode(CNTimeSlice obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        public static IEqualityComparer<CNTimeSlice> StartDateTimeEndDateTimeComparer { get; } = new StartDateTimeEndDateTimeEqualityComparer();

        /// <summary>
        /// must have start date to create a timeslice
        /// </summary>
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public string StartDate => StartDateTime.ToString(CNConstants.DIRECTORY_DATEFORMAT);
        public CNTimeSlice(DateTime startDateTime, DateTime? endDateTime = null)
        {
            StartDateTime = startDateTime;
            if (endDateTime != null && endDateTime <= startDateTime)
            {
                throw new ArgumentException("A TimeSlice's EndTime Must Greater Than StartTime");
            }
            EndDateTime = endDateTime;
        }


        public bool Equals(CNTimeSlice other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartDateTime.Equals(other.StartDateTime) && EndDateTime.Equals(other.EndDateTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CNTimeSlice) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StartDateTime.GetHashCode() * 397) ^ EndDateTime.GetHashCode();
            }
        }

        public object Clone()
        {
            return new CNTimeSlice(StartDateTime, EndDateTime);
        }
        /// <summary>
        /// Two TimeSlices InterceptWith Another:
        /// 1. Both dont have EndTime
        /// </summary>
        /// <param name="timeSlice"></param>
        /// <returns></returns>
        public bool InterceptWith(CNTimeSlice timeSlice)
        {
            if (timeSlice == null) return false;
            if (EndDateTime == null && timeSlice.EndDateTime == null)
            {
                return true;
            }
            else if (EndDateTime == null || timeSlice.EndDateTime == null)
            {
                var endtime = EndDateTime ?? timeSlice.EndDateTime;
                var starttime = EndDateTime == null ? StartDateTime : timeSlice.StartDateTime;
                return !endtime.HasValue || endtime.Value >= starttime;
            }
            else
            {
                if (EndDateTime.Value <= timeSlice.StartDateTime)
                    return false;
                if (timeSlice.EndDateTime.Value <= StartDateTime)
                    return false;
                return true;
            }
        }

        public int CompareTo(CNTimeSlice other)
        {
            if (this.StartDateTime > other.StartDateTime) return 1;
            else if (this.StartDateTime.Equals(other.StartDateTime)) return 0;
            return -1;
        }

        public override string ToString()
        {
            return $"{nameof(StartDateTime)}: {StartDateTime}, {nameof(EndDateTime)}: {EndDateTime}";
        }

        private CNTimeSlice dayDuration;
        /// <summary>
        /// Change current timeslice to normalized form like yyyy-MM-dd 00:00:00 - yyyy-MM-dd+N 23:59:59.999
        /// </summary>
        /// <returns></returns>
        public CNTimeSlice GetDayDuration()
        {
            if (StartDateTime > DateTime.Now) return null;
            if (dayDuration == null)
            {
                dayDuration = new CNTimeSlice(StartDateTime.Date,
                    ((EndDateTime ?? DateTime.Now).Date > StartDateTime.Date) ? 
                        (EndDateTime ?? DateTime.Now).Date.AddDays(1).AddMilliseconds(-1)
                        : StartDateTime.Date.AddDays(1).AddMilliseconds(-1));
            }
            return dayDuration;
        }
        /// <summary>
        /// Means otherTimeSlice is a SubSet of this timeSlice, if a equals b means they contain each other
        /// </summary>
        /// <param name="otherTimeSlice"></param>
        /// <returns></returns>
        public bool IsContaining(CNTimeSlice otherTimeSlice)
        {
            if (!this.EndDateTime.HasValue || !otherTimeSlice.EndDateTime.HasValue) return false;
            return this.StartDateTime <= otherTimeSlice.StartDateTime &&
                   this.EndDateTime.Value >= otherTimeSlice.EndDateTime.Value;
        }
        /// <summary>
        /// Get timeSlice's using days,start today end tomorrow means two days
        /// </summary>
        /// <returns></returns>
        public int GetWorkDays()
        {
            if(EndDateTime == null)return 0;
            return Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((EndDateTime.Value - StartDateTime).TotalDays)));
        }
    }

}
