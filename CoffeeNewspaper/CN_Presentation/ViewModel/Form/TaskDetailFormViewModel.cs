using System;
using CN_Core;
using CN_Core.Utilities;
using CN_Presentation.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Form
{
    public class TaskDetailFormViewModel : BaseViewModel, IUpdateDateTime
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
                    : (time.Value - DateTime.Now).GetTimeSpanLeftInfo();
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

        #endregion
    }
}