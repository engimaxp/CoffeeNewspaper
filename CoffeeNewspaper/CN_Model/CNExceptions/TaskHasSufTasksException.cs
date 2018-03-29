using System;
using System.Runtime.Serialization;

namespace CN_Model
{
    public class TaskHasSufTasksException : Exception
    {
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
    }
}
