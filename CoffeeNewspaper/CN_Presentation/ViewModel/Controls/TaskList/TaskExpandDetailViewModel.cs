using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Core.Utilities;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskExpandDetailViewModel : BaseViewModel
    {
        #region Private Methods

        /// <summary>
        ///     DFS to Add all child tasks
        /// </summary>
        /// <param name="taskInfoChildTasks"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private ObservableCollection<TaskTreeItemViewModel> MapToViewModel(ICollection<CNTask> taskInfoChildTasks,
            ITreeNodeSubscribe container)
        {
            var result = new ObservableCollection<TaskTreeItemViewModel>();
            foreach (var a in taskInfoChildTasks)
            {
                var viewModel =
                    new TaskTreeItemViewModel(container, a) {ChildItems = MapToViewModel(a.ChildTasks, container)};
                result.Add(viewModel);
            }

            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Child tasks viewModel to represent child task tree
        /// </summary>
        public TaskTreeViewModel ChildTasksModel { get; set; }

        /// <summary>
        ///     Task Content Detail
        /// </summary>
        public string TaskDetailContent { get; set; }

        /// <summary>
        ///     Created Time of this task
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        ///     Long Form is yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string CreatedTimeLongForm => $"({CreatedTime.ToString(CNConstants.DIRECTORY_DATETIMEFORMAT)})";

        /// <summary>
        ///     Short Form is like 'Yesterday','Early this Day',etc
        /// </summary>
        public string CreatedTimeShortForm => CreatedTime.EarlyTimeFormat();

        /// <summary>
        ///     Priority Control ViewModel
        /// </summary>
        public RatingViewModel PriorityRating { get; set; }

        /// <summary>
        ///     Urgency Control ViewModel
        /// </summary>
        public RatingViewModel UrgencyRating { get; set; }

        /// <summary>
        ///     Worked Duration display on the detail
        /// </summary>
        public string WorkedDuration { get; set; }

        /// <summary>
        ///     Estimated Time Left display on the detail
        ///     Meaning:Estimated Time - Worked Duration
        /// </summary>
        public string EstimatedTimeLeft { get; set; }

        /// <summary>
        ///     Time Remain Until DeadLine display on the detail
        ///     Meaning:DeadLine - now
        /// </summary>
        public string DeadLineTimeLeft { get; set; }

        public bool DisplayEstimatedTimeLeft => !string.IsNullOrEmpty(EstimatedTimeLeft);

        public bool DisplayDeadLineTimeLeft => !string.IsNullOrEmpty(DeadLineTimeLeft);

        #endregion

        #region Constructor

        public TaskExpandDetailViewModel()
        {
        }

        public TaskExpandDetailViewModel(CNTask TaskInfo)
        {
            if (TaskInfo == null) return;

            //Child Tasks
            var childTaskModel = new TaskTreeViewModel(TaskInfo);
            childTaskModel.Items = MapToViewModel(TaskInfo.ChildTasks, childTaskModel);
            ChildTasksModel = childTaskModel;

            //Content
            TaskDetailContent = TaskInfo.Content;

            //Created Time
            CreatedTime = TaskInfo.CreateTime;

            //Rating Controls
            UrgencyRating = RatingControlType.Urgency.GetNewModel(
                Enum.GetNames(typeof(CNUrgency)).ToList().IndexOf(TaskInfo.Urgency.ToString()) + 1,
                new RateChangedSubsriber(async x =>
                {
                    Enum.TryParse(Enum.GetNames(typeof(CNUrgency))[x - 1], out CNUrgency urgency);
                    if (TaskInfo.Urgency != urgency)
                    {
                        TaskInfo.Urgency = urgency;
                        await IoC.Get<ITaskService>().EditATask(TaskInfo);

                        await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
                    }
                })
            );
            PriorityRating = RatingControlType.Priority.GetNewModel(
                Enum.GetNames(typeof(CNPriority)).ToList().IndexOf(TaskInfo.Priority.ToString()) + 1,
                new RateChangedSubsriber(async x =>
                {
                    Enum.TryParse(Enum.GetNames(typeof(CNPriority))[x - 1], out CNPriority priority);
                    if (TaskInfo.Priority != priority)
                    {
                        TaskInfo.Priority = priority;
                        await IoC.Get<ITaskService>().EditATask(TaskInfo);

                        await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
                    }
                })
            );

            //Worked Duration
            var totalWorkedDays = TaskInfo.UsedTimeSlices.AsEnumerable().GetTotalDays();
            var totalWorkedHours = TaskInfo.UsedTimeSlices.AsEnumerable().GetTotalHours();
            WorkedDuration = totalWorkedDays <= 1
                ? $"{Convert.ToInt32(totalWorkedHours)} Hours"
                : $"{Convert.ToInt32(totalWorkedDays)} Day";

            //Estimated Time Left
            var workedDuration = Convert.ToInt64(totalWorkedHours * 3600);
            if (TaskInfo.EstimatedDuration > 0 && workedDuration < TaskInfo.EstimatedDuration)
                EstimatedTimeLeft =
                    new TimeSpan((TaskInfo.EstimatedDuration - workedDuration) * CNConstants.OneSecondToTickUnit)
                        .GetTimeSpanLeftInfo(false);

            //DeadLine Left
            if (TaskInfo.DeadLine.HasValue)
                DeadLineTimeLeft = DateTime.Now < TaskInfo.DeadLine.Value
                    ? (TaskInfo.DeadLine.Value - DateTime.Now).GetTimeSpanLeftInfo(false)
                    : "Over Due";
        }

        #endregion
    }
}