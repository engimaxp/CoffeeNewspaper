using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CN_Core
{
    /// <summary>
    ///     Throw this exception when Task's <see cref="CNTaskStatus" /> is not Right
    /// </summary>
    [Serializable]
    public class TaskStatusException : Exception
    {
        #region Construction

        public TaskStatusException(List<CNTaskStatus> targetTaskStatus, CNTaskStatus originStatus)
        {
            GetShoudBeStatus = targetTaskStatus;
            currentStatus = originStatus;
        }

        protected TaskStatusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Properties

        public List<CNTaskStatus> GetShoudBeStatus { get; }

        #endregion

        #region Formatting implementation

        public override string ToString()
        {
            return $"{nameof(GetShoudBeStatus)}: {GetShoudBeStatus}, {nameof(currentStatus)}: {currentStatus}";
        }

        #endregion

        #region Private Properties

        private readonly CNTaskStatus currentStatus;

        #endregion
    }
}