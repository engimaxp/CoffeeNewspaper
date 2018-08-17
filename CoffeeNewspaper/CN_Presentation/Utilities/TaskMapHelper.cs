using System;
using System.Linq;
using CN_Core;

namespace CN_Presentation.Utilities
{
    public static class TaskMapHelper
    {
        public static TimeSpan GetLastStartWorkTimeSpan(this CNTask task)
        {
            if (task == null || !task.UsedTimeSlices.Any()) return TimeSpan.Zero;
            var ts = task.UsedTimeSlices.ToList();
            ts.Sort();
            if (DateTime.Now < ts.Last().StartDateTime) return TimeSpan.Zero;
            return DateTime.Now - ts.Last().StartDateTime;
        }

        public static TaskUrgency MapFourQuadrantTaskUrgency(this CNTask task)
        {
            if ((int) task.Urgency > (int)CNUrgency.Normal && (int)task.Priority > (int)CNPriority.Normal)
            {
                return TaskUrgency.VeryUrgent;
            }else if ((int) task.Urgency < (int) CNUrgency.Normal && (int) task.Priority < (int) CNPriority.Normal)
            {
                return TaskUrgency.NotUrgent;
            }
            else
            {
                return TaskUrgency.Urgent;
            }
        }

        public static TaskCurrentStatus MapTaskCurrentStatus(this CNTask task)
        {
            if (task.IsFail)
            {
                return TaskCurrentStatus.DELETE;
            }

            switch (task.Status)
            {
                case CNTaskStatus.DOING:
                    return TaskCurrentStatus.UNDERGOING;
                case CNTaskStatus.DONE:
                    return TaskCurrentStatus.COMPLETE;
                case CNTaskStatus.PENDING:
                    return TaskCurrentStatus.PENDING;
                case CNTaskStatus.TODO:
                    return TaskCurrentStatus.STOP;
                default: return TaskCurrentStatus.STOP;
            }
        }
    }
}