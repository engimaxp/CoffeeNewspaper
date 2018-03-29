using System;
using System.Runtime.Serialization;

namespace CN_Model
{
    public class TaskHasChildTasksException : Exception
    {
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
    }
}
