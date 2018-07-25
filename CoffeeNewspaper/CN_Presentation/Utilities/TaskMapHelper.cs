using CN_Core;

namespace CN_Presentation.Utilities
{
    public static class TaskMapHelper
    {
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