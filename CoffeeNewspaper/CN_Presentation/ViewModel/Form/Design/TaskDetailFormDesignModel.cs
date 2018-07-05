using CN_Core;

namespace CN_Presentation.ViewModel.Form.Design
{
    public class TaskDetailFormDesignModel : TaskDetailFormViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TaskDetailFormDesignModel Instance => new TaskDetailFormDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TaskDetailFormDesignModel()
        {
            Status = TaskCurrentStatus.PENDING;
            IsFail = false;
            FailReason = "Due To blablabla this task is fail";
            PendingReason = "Due To blablabla this task is pending";
        }

        #endregion
    }
}