using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CN_Model
{
    public class TaskStatusException:Exception
    {
        private readonly List<CNTaskStatus> shoudBeStatus;
        private readonly CNTaskStatus currentStatus;

        public TaskStatusException(List<CNTaskStatus> targetTaskStatus, CNTaskStatus originStatus)
        {
            shoudBeStatus = targetTaskStatus;
            currentStatus = originStatus;
        }

        public override string ToString()
        {
            return $"{nameof(shoudBeStatus)}: {shoudBeStatus}, {nameof(currentStatus)}: {currentStatus}";
        }
        public List<CNTaskStatus> GetShoudBeStatus => shoudBeStatus;
    }
}
