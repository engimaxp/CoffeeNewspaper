using CN_Core;
using CN_Presentation.Input.Design;

namespace CN_Presentation.ViewModel.Form.Design
{
    public class TaskDetailFormDesignModel : TaskDetailFormViewModel
    {
        #region Constructor

        /// <summary>
        ///     Default Constructor
        /// </summary>
        public TaskDetailFormDesignModel():base(null)
        {
            Status = TaskCurrentStatus.PENDING;
            IsFail = true;
            FailReason = "Due To blablabla this task is fail";
            PendingReason = "Due To blablabla this task is pending";
            Content =
                "Each control in WPF has a DataContext property. \r\nIt's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree.";
        }

        #endregion

        #region Singleton

        /// <summary>
        ///     A single instance of the design model
        /// </summary>
        public static TaskDetailFormDesignModel Instance => new TaskDetailFormDesignModel();

        #endregion
    }
}