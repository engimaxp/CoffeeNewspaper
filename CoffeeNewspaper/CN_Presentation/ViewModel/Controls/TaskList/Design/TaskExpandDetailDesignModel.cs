using System;

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
            ChildTasksModel = TaskTreeDesignModel.Instance;
            TaskDetailContent =
                "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree.";
            CreatedTime = DateTime.Now.AddDays(-1);
            UrgencyRating = RatingDesignModel2.Instance;
            PriorityRating = RatingDesignModel.Instance;
        }
        #endregion
    }

}