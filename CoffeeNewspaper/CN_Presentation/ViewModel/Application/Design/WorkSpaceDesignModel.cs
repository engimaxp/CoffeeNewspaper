using CN_Core;
using CN_Presentation.ViewModel.Application;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class WorkSpaceDesignModel : WorkSpaceViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static WorkSpaceDesignModel Instance => new WorkSpaceDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WorkSpaceDesignModel()
        {
            TasksListViewModel = TaskListDesignModel.Instance;
        }

        #endregion
    }

    
}
