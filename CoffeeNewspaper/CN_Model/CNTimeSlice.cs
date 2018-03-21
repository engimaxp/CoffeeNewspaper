﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNTimeSlice:IEquatable<CNTimeSlice>,ICloneable
    {
        /// <summary>
        /// must have start date to create a timeslice
        /// </summary>
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public string StartDate => this.StartDateTime.ToString(CNConstants.DIRECTORY_DATEFORMAT);

        public CNTimeSlice(DateTime startDateTime, DateTime? endDateTime = null)
        {
            StartDateTime = startDateTime;
            if (endDateTime != null && endDateTime <= startDateTime)
            {
                throw new ArgumentException("A TImeSlice's EndTime Must Greater Than StartTime");
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
            if (obj.GetType() != this.GetType()) return false;
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
            if (this.EndDateTime == null && timeSlice.EndDateTime == null)
            {
                return true;
            }
            else if (this.EndDateTime == null || timeSlice.EndDateTime == null)
            {
                var endtime = this.EndDateTime ?? timeSlice.EndDateTime;
                var starttime = this.EndDateTime == null ? this.StartDateTime : timeSlice.StartDateTime;
                return !endtime.HasValue || endtime.Value >= starttime;
            }
            else
            {
                if (this.EndDateTime.Value <= timeSlice.StartDateTime)
                    return false;
                if (timeSlice.EndDateTime.Value <= this.StartDateTime)
                    return false;
                return true;
            }
        }
    }

}
