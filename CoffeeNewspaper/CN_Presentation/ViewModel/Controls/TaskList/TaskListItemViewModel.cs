using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Controls
{
    /// <summary>
    ///     ViewModel for TaskList item
    /// </summary>
    public class TaskListItemViewModel : BaseViewModel
    {
        #region Private Properties

        private CNTask _taskInfo;

        #endregion

        #region Constructor

        public TaskListItemViewModel(CNTask TaskInfo = null)
        {
            ExpandTaskCommand = new RelayCommand(ExpandTask);
            OpenEditDialogCommand = new RelayCommand(OpenEditDialog);
            this.TaskInfo = TaskInfo;
            if (TaskInfo != null)
            {
                TaskTitle = TaskInfo.Content.GetFirstLineOrWords(50);
                Urgency = TaskInfo.MapFourQuadrantTaskUrgency();
                Status = TaskInfo.MapTaskCurrentStatus();
            }
        }

        #endregion

        #region Public Propertie

        /// <summary>
        ///     How urgent a task is
        /// </summary>
        public TaskUrgency Urgency { get; set; }

        /// <summary>
        ///     What status is this task
        /// </summary>
        public TaskCurrentStatus Status { get; set; }

        /// <summary>
        ///     Title of this task
        /// </summary>
        public string TaskTitle { get; set; }

        /// <summary>
        ///     true if the detail of this task is reveled
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        ///     Store TaskInfo for future use
        /// </summary>
        public CNTask TaskInfo
        {
            get => _taskInfo;
            set
            {
                Refreshed = true;
                _taskInfo = value;
                if (_taskInfo != null)
                {
                    TaskTitle = _taskInfo.Content.GetFirstLineOrWords(50);
                    Urgency = _taskInfo.MapFourQuadrantTaskUrgency();
                    Status = _taskInfo.MapTaskCurrentStatus();
                }
            }
        }

        /// <summary>
        /// Every time refresh list where refresh this boolean to true
        /// after refresh every false items will be removed
        /// </summary>
        public bool Refreshed { get; set; }

        #endregion

        #region Commands

        /// <summary>
        ///     Expand the Item
        /// </summary>
        public ICommand ExpandTaskCommand { get; set; }

        /// <summary>
        ///     Open edit Dialog for this Task
        /// </summary>
        public ICommand OpenEditDialogCommand { get; set; }

        #endregion

        #region Private Methods

        private void ExpandTask()
        {
            IsExpanded ^= true;
        }

        private void OpenEditDialog()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Edit Task",
                FormContentViewModel = new TaskDetailFormViewModel(TaskInfo),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }

        #endregion
    }
}