namespace CN_Presentation.ViewModel.Controls.Design
{
    public class TaskListItemDesignModel:TaskListItemViewModel
    {

        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TaskListItemDesignModel Instance => new TaskListItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskListItemDesignModel()
        {
            Urgency = TaskUrgency.NotUrgent;
            IsExpanded = true;
            Status = TaskCurrentStatus.COMPLETE;
            TaskTitle =
                "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree.";
        }
        #endregion
    }

}