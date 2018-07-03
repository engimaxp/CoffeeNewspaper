namespace CN_Presentation.ViewModel.Controls.Design
{
    public class TaskExpandDetailDesignModel : TaskExpandDetailViewModel
    {

        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TaskExpandDetailDesignModel Instance => new TaskExpandDetailDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskExpandDetailDesignModel()
        {
        }
        #endregion
    }

}