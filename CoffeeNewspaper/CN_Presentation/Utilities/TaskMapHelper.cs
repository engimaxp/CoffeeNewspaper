using System;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Service;

namespace CN_Presentation.Utilities
{
    public static class TaskMapHelper
    {
        public static async Task<TimeSpan> GetLastStartWorkTimeSpan(this CNTask task)
        {
            if(task == null || task.TaskId<=0) return TimeSpan.Zero;
            var timeslices = await IoC.Get<ITaskService>().GetTaskTimeSlices(task.TaskId);
            if (!timeslices.Any()) return TimeSpan.Zero;
            var ts = timeslices.ToList();
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