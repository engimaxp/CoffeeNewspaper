using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        /// <param name="selectedTaskId"></param>
        /// <returns></returns>
        private ObservableCollection<TaskTreeItemViewModel> MapToViewModel(IOrderedEnumerable<CNTask> taskInfoChildTasks,
            ITreeNodeSubscribe container,int? selectedTaskId)
        {
            var result = new ObservableCollection<TaskTreeItemViewModel>();
            foreach (var a in taskInfoChildTasks)
            {
                var viewModel =
                    new TaskTreeItemViewModel(container, a)
                    {
                        IsSelected = a.TaskId == (selectedTaskId ?? 0),
                        ChildItems = MapToViewModel(a.ChildTasks.FilterDeletedAndOrderBySortTasks(), container,selectedTaskId)
                    };
                result.Add(viewModel);
            }

            return result;
        }

        private static RateChangedSubsriber PriorityRateChangedEvent(CNTask taskInfo)
        {
            return new RateChangedSubsriber(async x =>
            {
                Enum.TryParse(Enum.GetNames(typeof(CNPriority))[x - 1], out CNPriority priority);
                if (taskInfo.Priority != priority)
                {
                    taskInfo.Priority = priority;
                    await IoC.Get<ITaskService>().EditATask(taskInfo);

                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(taskInfo.TaskId);
                }
            });
        }

        private static RateChangedSubsriber UrgencyRateChangedEvent(CNTask taskInfo)
        {
            return new RateChangedSubsriber(async x =>
            {
                Enum.TryParse(Enum.GetNames(typeof(CNUrgency))[x - 1], out CNUrgency urgency);
                if (taskInfo.Urgency != urgency)
                {
                    taskInfo.Urgency = urgency;
                    await IoC.Get<ITaskService>().EditATask(taskInfo);

                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(taskInfo.TaskId);
                }
            });
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

        /// <summary>
        /// Whether Display the block of EstimatedTimeLeft
        /// </summary>
        public bool DisplayEstimatedTimeLeft => !string.IsNullOrEmpty(EstimatedTimeLeft);

        /// <summary>
        /// Whether Display the block of DeadLineTimeLeft
        /// </summary>
        public bool DisplayDeadLineTimeLeft => !string.IsNullOrEmpty(DeadLineTimeLeft);

        /// <summary>
        /// Fail Reason of this Task
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        /// Whether Display the block of FailReason
        /// </summary>
        public bool DisplayFailReason => !string.IsNullOrEmpty(FailReason);

        /// <summary>
        /// Pending Reason of this Task
        /// </summary>
        public string PendingReason { get; set; }

        /// <summary>
        /// Whether Display the block of PendingReason
        /// </summary>
        public bool DisplayPendingReason => !string.IsNullOrEmpty(PendingReason);

        /// <summary>
        ///     Display Tag Panel ViewModels
        /// </summary>
        public TagPanelViewModel TagPanelViewModel { get; set; } = new TagPanelViewModel();
        #endregion

        #region Constructor

        public TaskExpandDetailViewModel()
        {
        }

        public TaskExpandDetailViewModel(CNTask taskInfo,int? selectChildTaskId)
        {
            if (taskInfo == null) return;

            //Child Tasks
            var childTaskModel = new TaskTreeViewModel(taskInfo);
            childTaskModel.Items = MapToViewModel(taskInfo.ChildTasks.FilterDeletedAndOrderBySortTasks(), childTaskModel, selectChildTaskId);
            ChildTasksModel = childTaskModel;

            //Content
            TaskDetailContent = taskInfo.Content;

            //Created Time
            CreatedTime = taskInfo.CreateTime;

            //Rating Controls
            UrgencyRating = RatingControlType.Urgency.GetNewModel(
                Enum.GetNames(typeof(CNUrgency)).ToList().IndexOf(taskInfo.Urgency.ToString()) + 1,
                UrgencyRateChangedEvent(taskInfo)
            );
            PriorityRating = RatingControlType.Priority.GetNewModel(
                Enum.GetNames(typeof(CNPriority)).ToList().IndexOf(taskInfo.Priority.ToString()) + 1,
                PriorityRateChangedEvent(taskInfo)
            );

            //Worked Duration
            var totalWorkedDays = taskInfo.UsedTimeSlices.AsEnumerable().GetTotalDays();
            var totalWorkedHours = taskInfo.UsedTimeSlices.AsEnumerable().GetTotalHours();
            WorkedDuration = totalWorkedDays <= 1
                ? $"{Convert.ToInt32(totalWorkedHours)} Hours"
                : $"{Convert.ToInt32(totalWorkedDays)} Day";

            //Estimated Time Left
            var workedDuration = Convert.ToInt64(totalWorkedHours * 3600);
            if (taskInfo.EstimatedDuration > 0 && workedDuration < taskInfo.EstimatedDuration)
                EstimatedTimeLeft =
                    new TimeSpan((taskInfo.EstimatedDuration - workedDuration) * CNConstants.OneSecondToTickUnit)
                        .GetTimeSpanLeftInfo(false);

            //DeadLine Left
            if (taskInfo.DeadLine.HasValue)
                DeadLineTimeLeft = DateTime.Now < taskInfo.DeadLine.Value
                    ? (taskInfo.DeadLine.Value - DateTime.Now).GetTimeSpanLeftInfo(false)
                    : "Over Due";

            //Fail Reason
            if (taskInfo.IsFail)
                FailReason = string.IsNullOrEmpty(taskInfo.FailReason) ? "No description" : taskInfo.FailReason;

            //Pending Reason
            if (taskInfo.Status == CNTaskStatus.PENDING)
                PendingReason = string.IsNullOrEmpty(taskInfo.PendingReason)
                    ? "No description"
                    : taskInfo.PendingReason;

            //Tag Panels
            TagPanelViewModel.TagItems = new ObservableCollection<TagItemViewModel>(taskInfo.TaskTaggers
                .Select(x => x.Tag)
                .Select(y => new TagItemViewModel(TagPanelViewModel)
                {
                    TagId = y.TagId,
                    TagTitle = y.Title
                }));
        }

        #endregion
    }
}