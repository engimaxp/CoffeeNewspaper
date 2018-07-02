using System;
using System.Runtime.Serialization;

namespace CN_Core
{
    /// <summary>
    ///     This <see cref="CNTask" /> has suffix tasks
    ///     throw this exception while try to remove or delete a task
    /// </summary>
    [Serializable]
    public class TaskHasSufTasksException : Exception
    {
        #region Construction

        public TaskHasSufTasksException()
        {
        }

        public TaskHasSufTasksException(string message) : base(message)
        {
        }

        public TaskHasSufTasksException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaskHasSufTasksException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}