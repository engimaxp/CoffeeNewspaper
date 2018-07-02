using System;
using System.Runtime.Serialization;

namespace CN_Core
{
    /// <summary>
    ///     This <see cref="CNTimeSlice" /> Intervene with another 
    /// </summary>
    [Serializable]
    public class TimeSliceInterveneException : Exception
    {
        #region Construction

        public TimeSliceInterveneException()
        {
        }

        public TimeSliceInterveneException(string message) : base(message)
        {
        }

        public TimeSliceInterveneException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TimeSliceInterveneException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}