﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Core.Utilities;
using CN_Presentation.Input;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.ViewModel.Form
{
    public class TaskDetailFormViewModel : FormBaseViewModel, IUpdateDateTime, IUpdateTimeRange
    {
        #region Private Properties

        private readonly CNTask originTask;

        #endregion

        #region Base Confirm Action Implement

        public override async Task<bool> Confirm()
        {
            return await RunCommandAsyncGeneric(() => ConfirmIsRunning, async () =>
            {
                var result = false;
                var newTask = GenerateCNTask();
                await TaskOperatorHelper.WrapException(async () =>
                {
                    if (originTask == null)
                        result = (await IoC.Get<ITaskService>().CreateATask(newTask))?.TaskId > 0;
                    else
                        result = await IoC.Get<ITaskService>().EditATask(newTask);
                });
                await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(newTask.TaskId);
                return result;
            });
        }

        #endregion

        #region Private Methods

        private CNTask GenerateCNTask()
        {
            var result = new CNTask
            {
                CreateTime = DateTime.Now,
                ParentTaskID = ParentTaskId
            };
            if (originTask != null && originTask.TaskId > 0) result = originTask;

            result.Content = Content;
            result.DeadLine = DeadLineEntry.SelectedDateTime;
            result.EstimatedDuration = EstimatedDurationEntry.SelectedTimeDuration;

            Enum.TryParse(Enum.GetNames(typeof(CNUrgency))[UrgencyRating.SelectedValue - 1], out CNUrgency urgency);
            result.Urgency = urgency;

            Enum.TryParse(Enum.GetNames(typeof(CNPriority))[PriorityRating.SelectedValue - 1], out CNPriority priority);
            result.Priority = priority;


            //Delete Tags
            if (result.TaskTaggers.Count > 0)
            {
                var remainOldTags = TagPanelViewModel.TagItems.Where(x => !string.IsNullOrEmpty(x.TagId))
                    .Select(y => y.TagId);
                foreach (var cnTaskTagger in result.TaskTaggers.Where(x => !remainOldTags.Contains(x.TagId)).ToList())
                    result.TaskTaggers.Remove(cnTaskTagger);
            }

            //Add Tags
            foreach (var tagItemViewModel in TagPanelViewModel.TagItems)
            {
                //Jump over old tags
                if (result.TaskTaggers.Where(x => !string.IsNullOrEmpty(x.TagId)).Any(x =>
                        x.TagId == tagItemViewModel.TagId))
                    continue;

                var curTaskTagger = string.IsNullOrEmpty(tagItemViewModel.TagId)
                    ? new CNTaskTagger
                    {
                        Task = result,
                        Tag = new CNTag
                        {
                            Title = tagItemViewModel.TagTitle
                        }
                    }
                    : new CNTaskTagger
                    {
                        Task = result,
                        TagId = tagItemViewModel.TagId
                    };
                result.TaskTaggers.Add(curTaskTagger);
            }

            return result;
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor to create a task detail form view model
        /// </summary>
        /// <param name="originTask">Origin Task ,null if it's add a task</param>
        /// <param name="parentTaskId">Parent Task ,if it's adding a task and parentTask is not null then its refresh a Detail View</param>
        public TaskDetailFormViewModel(CNTask originTask, int parentTaskId = 0)
        {
            this.originTask = originTask;
            var dateTimeViewModel = new DateTimeEntryViewModel();
            dateTimeViewModel.SetParentInterface(this);
            DeadLineEntry = dateTimeViewModel;

            var timeRangeViewModel = new TimeRangeEntryViewModel();
            timeRangeViewModel.SetParentInterface(this);
            EstimatedDurationEntry = timeRangeViewModel;
            //Edit
            if (originTask != null)
            {
                ParentTaskTitle = originTask.ParentTask == null
                    ? "Empty"
                    : originTask.ParentTask.Content.GetFirstLineOrWords(50);
                Status = originTask.MapTaskCurrentStatus();
                IsFail = originTask.IsFail;
                FailReason = originTask.FailReason;
                PendingReason = originTask.PendingReason;
                Content = originTask.Content;
                DeadLineEntry.SelectedDateTime = originTask.DeadLine;
                EstimatedDurationEntry.SelectedTimeDuration = originTask.EstimatedDuration;
                UrgencyRating = RatingControlType.Urgency.GetNewModel(
                    Enum.GetNames(typeof(CNUrgency)).ToList().IndexOf(originTask.Urgency.ToString()) + 1);
                PriorityRating = RatingControlType.Priority.GetNewModel(
                    Enum.GetNames(typeof(CNPriority)).ToList().IndexOf(originTask.Priority.ToString()) + 1);
                TagPanelViewModel.TagItems = new ObservableCollection<TagItemViewModel>(originTask.TaskTaggers
                    .Select(x => x.Tag)
                    .Select(y => new TagItemViewModel(TagPanelViewModel)
                    {
                        TagId = y.TagId,
                        TagTitle = y.Title
                    }));
            }
            //Add New
            else
            {
                if (parentTaskId >0 )
                {
                    var task = Task.Run(async () => await IoC.Get<ITaskService>().GetTaskById(parentTaskId));
                    task.Wait();
                    if (task.Result != null)
                    {
                        ParentTaskTitle = task.Result.Content.GetFirstLineOrWords(50);
                        ParentTaskId = parentTaskId;
                        UrgencyRating = RatingControlType.Urgency.GetNewModel(
                            Enum.GetNames(typeof(CNUrgency)).ToList().IndexOf(task.Result.Urgency.ToString()) + 1);
                        PriorityRating = RatingControlType.Priority.GetNewModel(
                            Enum.GetNames(typeof(CNPriority)).ToList().IndexOf(task.Result.Priority.ToString()) + 1);
                        TagPanelViewModel.TagItems = new ObservableCollection<TagItemViewModel>(task.Result.TaskTaggers
                            .Select(x => x.Tag)
                            .Select(y => new TagItemViewModel(TagPanelViewModel)
                            {
                                TagId = y.TagId,
                                TagTitle = y.Title
                            }));
                    }
                }
                else
                {
                    ParentTaskTitle = "Empty";

                UrgencyRating = RatingControlType.Urgency.GetNewModel(3);
                    PriorityRating = RatingControlType.Priority.GetNewModel(3);
                }
            }

            IsEditing = true;
        }

        private int? ParentTaskId { get; }

        #endregion

        #region Public Methods

        #region Interface Implement

        public void NotifyUpdateDateTime(DateTime? time)
        {
            if (time == null)
                DeadLineLeft = string.Empty;
            else
                DeadLineLeft = time.Value < DateTime.Now
                    ? "Over Due"
                    : (time.Value - DateTime.Now).GetTimeSpanLeftInfo(true);
        }

        public void NotifyUpdateTimeRange(long timeRangeSeconds)
        {
            if (timeRangeSeconds > 0)
                UsedTimePercent = ((timeRangeSeconds - 300.0) / timeRangeSeconds).ToString("P2") + " Left";
            else UsedTimePercent = string.Empty;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Display Current Task Status
        /// </summary>
        public TaskCurrentStatus Status { get; set; }

        /// <summary>
        ///     If the task is fail ,then true
        /// </summary>
        public bool IsFail { get; set; }

        /// <summary>
        ///     If the task is currently pending ,then true
        ///     need this field to decide <see cref="PendingReason" /> display or not
        /// </summary>
        public bool IsPending => Status == TaskCurrentStatus.PENDING;

        /// <summary>
        ///     Fail reason of this task
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        ///     Pending reason of this task
        /// </summary>
        public string PendingReason { get; set; }

        /// <summary>
        ///     Task mainly content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Autofocus target TextBox
        /// </summary>
        public bool IsEditing { get; set; }

        /// <summary>
        ///     Title is the first line of the tasks content
        ///     it will cut space characters and show only 50 char
        /// </summary>
        public string Title => Content.GetFirstLineOrWords(50);

        /// <summary>
        ///     The Deadline Entry ViewModel to provide
        /// </summary>
        public DateTimeEntryViewModel DeadLineEntry { get; set; }


        /// <summary>
        ///     Mainly used to cooperate with <see cref="DeadLineEntry" />, to calculate the currently left time
        /// </summary>
        public string DeadLineLeft { get; set; }

        /// <summary>
        ///     Estimated Duration of this task
        /// </summary>
        public TimeRangeEntryViewModel EstimatedDurationEntry { get; set; }

        /// <summary>
        ///     Display Tag Panel ViewModels
        /// </summary>
        public TagPanelViewModel TagPanelViewModel { get; set; } = new TagPanelViewModel();

        /// <summary>
        ///     Mainly used to cooperate with <see cref="EstimatedDurationEntry" />. to calculate the currently work complete
        ///     percentage
        /// </summary>
        public string UsedTimePercent { get; set; }

        public RatingViewModel UrgencyRating { get; set; }

        public RatingViewModel PriorityRating { get; set; }

        /// <summary>
        ///     this task's parent task's title
        /// </summary>
        public string ParentTaskTitle { get; set; }

        #endregion
    }
}