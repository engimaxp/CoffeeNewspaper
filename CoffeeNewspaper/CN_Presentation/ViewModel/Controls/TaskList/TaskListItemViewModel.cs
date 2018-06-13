using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    /// <summary>
    /// ViewModel for TaskList item
    /// </summary>
    public class TaskListItemViewModel:BaseViewModel
    {
        #region Public Propertie

        /// <summary>
        /// How urgent a task is
        /// </summary>
        public TaskUrgency Urgency { get; set; }

        /// <summary>
        /// What status is this task
        /// </summary>
        public TaskCurrentStatus Status { get; set; }

        /// <summary>
        /// Title of this task
        /// </summary>
        public string TaskTitle { get; set; }

        /// <summary>
        /// true if the detail of this task is reveled
        /// </summary>
        public bool IsExpanded { get; set; }
        #endregion

        #region Commands

        /// <summary>
        /// Expand the Item
        /// </summary>
        public ICommand ExpandTaskCommand { get; set; }

        #endregion

        #region MyRegion

        public TaskListItemViewModel()
        {
            ExpandTaskCommand = new RelayCommand(ExpandTask);
        }

        #endregion
        #region Public Methods

        public void ExpandTask()
        {
            IsExpanded ^= true;
        }
        #endregion
    }
}
