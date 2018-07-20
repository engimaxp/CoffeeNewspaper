using System;
using CN_Core;
using CN_Core.Utilities;
using CN_Presentation.Input;
using CN_Presentation.Input.Design;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.ViewModel.Form
{
    public class TaskDetailFormViewModel : BaseViewModel, IUpdateDateTime, IUpdateTimeRange 
    {
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

        #endregion

    }
}