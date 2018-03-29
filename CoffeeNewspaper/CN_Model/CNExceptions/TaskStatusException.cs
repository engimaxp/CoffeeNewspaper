using System;
using System.Runtime.Serialization;

namespace CN_Model
{
    public class TaskStatusException:Exception
    {
        private readonly CNTaskStatus shoudBeStatus;
        private readonly CNTaskStatus currentStatus;

        public TaskStatusException(CNTaskStatus targetTaskStatus, CNTaskStatus originStatus)
        {
            shoudBeStatus = targetTaskStatus;
            currentStatus = originStatus;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(shoudBeStatus)}: {shoudBeStatus}, {nameof(currentStatus)}: {currentStatus}";
        }
        public CNTaskStatus GetShoudBeStatus => shoudBeStatus;
    }
}
