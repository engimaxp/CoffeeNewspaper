using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     A task's duration is a combination of its timeslices
    /// </summary>
    public class CNTimeSlice : IEquatable<CNTimeSlice>, ICloneable, IComparable<CNTimeSlice>
    {
        #region Private Properties

        /// <summary>
        ///     store the calculated day duration of this instance
        /// </summary>
        private CNTimeSlice dayDuration;

        #endregion

        #region Constructor

        /// <summary>
        ///     must have start date to create a timeslice
        /// </summary>
        /// <param name="startDateTime">start time of this slice</param>
        /// <param name="endDateTime">end time of this slice , null if not give a value</param>
        public CNTimeSlice(DateTime startDateTime, DateTime? endDateTime = null)
        {
            StartDateTime = startDateTime;
            if (endDateTime != null && endDateTime <= startDateTime)
                throw new ArgumentException("A TimeSlice's EndTime Must Greater Than StartTime");
            EndDateTime = endDateTime;
        }
        /// <summary>
        /// EF requires a parameterless constructor be declared
        /// </summary>
        public CNTimeSlice()
        {
            
        }
        #endregion

        #region Interface Implementation

        #region Clone implement

        public object Clone()
        {
            return new CNTimeSlice(StartDateTime, EndDateTime);
        }

        #endregion

        #region Comparable implement

        /// <summary>
        ///     compare two timeslice just by startDateTime
        /// </summary>
        /// <param name="other">the timeslice to compare</param>
        /// <returns>
        ///     1 if this StartTime greater than other StartTime
        ///     0 if its equal
        ///     -1 if its lesser
        /// </returns>
        public int CompareTo(CNTimeSlice other)
        {
            if (StartDateTime > other.StartDateTime) return 1;
            else if (StartDateTime.Equals(other.StartDateTime)) return 0;
            return -1;
        }

        #endregion

        #region Formatting implement

        public override string ToString()
        {
            return $"{nameof(StartDateTime)}: {StartDateTime}, {nameof(EndDateTime)}: {EndDateTime}";
        }

        #endregion

        #region Equality Implement
        

        private sealed class StartDateTimeEndDateTimeEqualityComparer : IEqualityComparer<CNTimeSlice>
        {
            public bool Equals(CNTimeSlice x, CNTimeSlice y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null) return false;
                if (y == null) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.StartDateTime.Equals(y.StartDateTime) && x.EndDateTime.Equals(y.EndDateTime);
            }

            public int GetHashCode(CNTimeSlice obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        public static IEqualityComparer<CNTimeSlice> StartDateTimeEndDateTimeComparer { get; } =
            new StartDateTimeEndDateTimeEqualityComparer();

        public bool Equals(CNTimeSlice other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartDateTime.Equals(other.StartDateTime) && EndDateTime.Equals(other.EndDateTime);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
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

        #endregion

        #region Operator Overide
        
        public static bool operator ==(CNTimeSlice left, CNTimeSlice right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left.Equals(right);
        }
        public static bool operator >(CNTimeSlice left, CNTimeSlice right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator <(CNTimeSlice left, CNTimeSlice right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >=(CNTimeSlice left, CNTimeSlice right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator <=(CNTimeSlice left, CNTimeSlice right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator !=(CNTimeSlice left, CNTimeSlice right)
        {
            return !(left == right);
        }

        #endregion
        #endregion

        #region Public Properties

        /// <summary>
        ///     Id of this timeSlice entity
        /// </summary>
        public string TimeSliceId { get; set; }

        /// <summary>
        ///     Start time of this slice
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        ///     End time of this slice
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        ///     The id of which task this slice belongs to
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     Which task this slice belongs to
        /// </summary>
        public virtual CNTask Task { get; set; }
        
        #endregion

        #region Public Methods

        /// <summary>
        ///     Change current timeslice to normalized form like yyyy-MM-dd 00:00:00 - yyyy-MM-dd+N 23:59:59.999
        /// </summary>
        /// <returns></returns>
        public CNTimeSlice GetDayDuration()
        {
            if (StartDateTime > DateTime.Now) return null;
            return dayDuration ??
                   (dayDuration = new CNTimeSlice(StartDateTime.Date,
                       (EndDateTime ?? DateTime.Now).Date > StartDateTime.Date
                           ? (EndDateTime ?? DateTime.Now).Date.AddDays(1).AddMilliseconds(-1)
                           : StartDateTime.Date.AddDays(1).AddMilliseconds(-1)));
        }

        /// <summary>
        ///     Means otherTimeSlice is a SubSet of this timeSlice, if a equals b means they contain each other
        /// </summary>
        /// <param name="otherTimeSlice"></param>
        /// <returns>true if this instance contain <see cref="otherTimeSlice" /></returns>
        public bool IsContaining(CNTimeSlice otherTimeSlice)
        {
            if (!EndDateTime.HasValue || !otherTimeSlice.EndDateTime.HasValue) return false;
            return StartDateTime <= otherTimeSlice.StartDateTime &&
                   EndDateTime.Value >= otherTimeSlice.EndDateTime.Value;
        }

        /// <summary>
        ///     Get timeSlice's using days,start today end tomorrow means two days
        /// </summary>
        /// <returns>the days this slice encontered</returns>
        public int GetWorkDays()
        {
            if (EndDateTime == null) return 0;
            return Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((EndDateTime.Value - StartDateTime).TotalDays)));
        }

        /// <summary>
        ///     Two TimeSlices InterceptWith Another:
        ///     1. Both dont have EndTime
        ///     2. One has no EndTime and its starttime greater than the others endTime
        ///     3. One contains anoter
        ///     4. Not when one's startTime and endTime both lesser than another's StartTime
        /// </summary>
        /// <param name="timeSlice">the other timeslice</param>
        /// <returns>true if this instance intercept with <see cref="timeSlice" /></returns>
        public bool InterceptWith(CNTimeSlice timeSlice)
        {
            if (timeSlice == null) return false;
            // Both dont have EndTime
            if (EndDateTime == null && timeSlice.EndDateTime == null)
            {
                return true;
            }
            // One has no EndTime and its starttime bigger than the others endTime
            else if (EndDateTime == null || timeSlice.EndDateTime == null)
            {
                var endtime = EndDateTime ?? timeSlice.EndDateTime;
                var starttime = EndDateTime == null ? StartDateTime : timeSlice.StartDateTime;
                return !endtime.HasValue || endtime.Value >= starttime;
            }

            // One contains anoter
            // Not when one's startTime and endTime both lesser than another's StartTime
            if (EndDateTime.Value <= timeSlice.StartDateTime)
                return false;
            if (timeSlice.EndDateTime.Value <= StartDateTime)
                return false;

            // Other circumstances returns true
            return true;
        }

        #endregion
    }
}