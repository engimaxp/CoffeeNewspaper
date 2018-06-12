using System;
using System.Drawing;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Service;
using Console = Colorful.Console;

namespace CoffeeNewspaper_CLI
{
    public class TaskEditCommand : BaseCommand
    {
        public TaskEditCommand(BaseState state) : base(state)
        {
            Name = "edit";
        }

        public override async Task<BaseState> Excute(ArgumentParser input)
        {
            if (State != null && State.StateObj is CNTask task)
            {
                var content = input.GetSingle<string>("c");
                if (!string.IsNullOrEmpty(content)) task.Content = content;
                var deadline = input.GetSingle<DateTime>("d");
                if (deadline > DateTime.MinValue) task.DeadLine = deadline;
                var estimatedDuration = input.GetSingle<string>("e");
                if (!string.IsNullOrEmpty(estimatedDuration))
                    task.EstimatedDuration = GetDurationMinutes(estimatedDuration);
                var priority = input.GetSingle<int>("p");
                switch (priority)
                {
                    case 1:
                    {
                        task.Priority = CNPriority.VeryLow;
                        break;
                    }
                    case 2:
                    {
                        task.Priority = CNPriority.Low;
                        break;
                    }
                    case 3:
                    {
                        task.Priority = CNPriority.Normal;
                        break;
                    }
                    case 4:
                    {
                        task.Priority = CNPriority.High;
                        break;
                    }
                    case 5:
                    {
                        task.Priority = CNPriority.VeryHigh;
                        break;
                    }
                }

                var urgency = input.GetSingle<int>("u");
                switch (urgency)
                {
                    case 1:
                    {
                        task.Urgency = CNUrgency.VeryLow;
                        break;
                    }
                    case 2:
                    {
                        task.Urgency = CNUrgency.Low;
                        break;
                    }
                    case 3:
                    {
                        task.Urgency = CNUrgency.Normal;
                        break;
                    }
                    case 4:
                    {
                        task.Urgency = CNUrgency.High;
                        break;
                    }
                    case 5:
                    {
                        task.Urgency = CNUrgency.VeryHigh;
                        break;
                    }
                }

                int parentTaskId = input.GetSingle<int>("parent");
                if (parentTaskId > 0)
                {
                    var parentTask = await IoC.Get<ITaskService>().GetTaskById(parentTaskId);
                    await IoC.Get<ITaskService>().SetParentTask(task, parentTask);
                }
                State.Refresh();
            }
            else
            {
                Console.WriteLine("wrong parameter", Color.Red);
            }

            Console.WriteLine();
            return State;
        }

        private int GetDurationMinutes(string estimatedDuration)
        {
            if (string.IsNullOrEmpty(estimatedDuration)) return CNConstants.ESTIMATED_DURATION_NOTWRITE;
            if (estimatedDuration.Trim().EndsWith("m", StringComparison.CurrentCultureIgnoreCase))
            {
                var minutes = estimatedDuration.Trim().Replace("m", "");
                return Convert.ToInt32(minutes);
            }

            if (estimatedDuration.Trim().EndsWith("h", StringComparison.CurrentCultureIgnoreCase))
            {
                var hours = estimatedDuration.Trim().Replace("h", "");
                return Convert.ToInt32(Convert.ToDouble(hours) * 60);
            }

            if (estimatedDuration.Trim().EndsWith("d", StringComparison.CurrentCultureIgnoreCase))
            {
                var days = estimatedDuration.Trim().Replace("d", "");
                return Convert.ToInt32(Convert.ToDouble(days) * 24 * 60);
            }

            if (estimatedDuration.Trim().EndsWith("w", StringComparison.CurrentCultureIgnoreCase))
            {
                var weeks = estimatedDuration.Trim().Replace("w", "");
                return Convert.ToInt32(Convert.ToDouble(weeks) * 7 * 24 * 60);
            }

            return Convert.ToInt32(estimatedDuration.Trim());
        }

    }
}