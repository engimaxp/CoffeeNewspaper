using System;
using System.Runtime.Serialization;

namespace CN_Core
{
    /// <summary>
    ///     This <see cref="CNTask" /> has child tasks
    ///     throw this exception while try to remove or delete a task
    /// </summary>
    public class TaskHasChildTasksException : Exception
    {
        #region Construction

        public TaskHasChildTasksException()
        {
        }

        public TaskHasChildTasksException(string message) : base(message)
        {
        }

        public TaskHasChildTasksException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaskHasChildTasksException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}