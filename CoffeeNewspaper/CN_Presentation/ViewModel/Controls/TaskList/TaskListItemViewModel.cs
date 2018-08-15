using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
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
        #region Constructor

        public TaskListItemViewModel(CNTask TaskInfo = null)
        {
            ExpandTaskCommand = new RelayCommand(ExpandTask);
            OpenEditDialogCommand = new RelayCommand(OpenEditDialog);
            SelectTaskCommand  = new RelayCommand(SelectTask);
            DisplayMoreOpCommand = new RelayCommand(DisplayMoreOp);
            PendingCommand= new RelayCommand(Pending);
            FailingCommand = new RelayCommand(Failing);
            DeleteCommand = new RelayCommand(Delete);

            this.TaskInfo = TaskInfo;
            if (TaskInfo != null)
            {
                TaskTitle = TaskInfo.Content.GetFirstLineOrWords(50);
                Urgency = TaskInfo.MapFourQuadrantTaskUrgency();
                Status = TaskInfo.MapTaskCurrentStatus();
                CanPending = TaskInfo.Status != CNTaskStatus.PENDING && !TaskInfo.IsFail;
                CanFail = !TaskInfo.IsFail;
            }
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            Refreshed = true;
            if (TaskInfo != null)
            {
                TaskTitle = TaskInfo.Content.GetFirstLineOrWords(50);
                Urgency = TaskInfo.MapFourQuadrantTaskUrgency();
                Status = TaskInfo.MapTaskCurrentStatus();
                RefreshExpanderView();
            }
        }

        public void RefreshExpanderView(int? childTaskId = null)
        {
            if (IsExpanded)
            {
                LoadDataToDetailExpanderView(childTaskId);
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
        ///     true if the task is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        ///     true if more op is displayed
        /// </summary>
        public bool IsMoreOpDisplayed { get; set; }

        /// <summary>
        ///     true if task can be set pending
        /// </summary>
        public bool CanPending { get; set; }

        /// <summary>
        ///     true if task can be set fail
        /// </summary>
        public bool CanFail { get; set; }

        /// <summary>
        ///     Store TaskInfo for future use
        /// </summary>
        public CNTask TaskInfo { get; set; }

        /// <summary>
        ///     Every time refresh list where refresh this boolean to true
        ///     after refresh every false items will be removed
        /// </summary>
        public bool Refreshed { get; set; }

        /// <summary>
        ///     Expand Detail View Model
        /// </summary>
        public TaskExpandDetailViewModel ExpandDetailViewModel { get; set; } = new TaskExpandDetailViewModel();

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

        /// <summary>
        ///     Select a Task
        /// </summary>
        public ICommand SelectTaskCommand { get; set; }

        /// <summary>
        /// Toggle display More Op
        /// </summary>
        public ICommand DisplayMoreOpCommand { get; set; }

        /// <summary>
        /// Set task to pending
        /// </summary>
        public ICommand PendingCommand { get; set; }

        /// <summary>
        /// Set task to Fail
        /// </summary>
        public ICommand FailingCommand { get; set; }

        /// <summary>
        /// Delete target task
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Private Methods

        private void ExpandTask()
        {
            if (!IsExpanded)
            {
                LoadDataToDetailExpanderView();
            }
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

        private void SelectTask()
        {
            this.IsSelected = true;
            //other task deselct
            foreach (var model in IoC.Get<TaskListViewModel>().Items.
                Where(x=>x.TaskInfo?.TaskId!=this.TaskInfo?.TaskId && x.IsSelected))
            {
                model.IsSelected = false;
            }
        }

        private void DisplayMoreOp()
        {
            this.IsMoreOpDisplayed ^= true;
            //other task deselct
            if (IsMoreOpDisplayed)
            {
                foreach (var model in IoC.Get<TaskListViewModel>().Items.
                    Where(x => x.TaskInfo?.TaskId != this.TaskInfo?.TaskId && x.IsMoreOpDisplayed))
                {
                    model.IsMoreOpDisplayed = false;
                }
            }
        }

        private void LoadDataToDetailExpanderView(int? childTaskId = null)
        {
            ExpandDetailViewModel = new TaskExpandDetailViewModel(TaskInfo, childTaskId);
        }

        private void Pending()
        {
            IsMoreOpDisplayed = false;
            if (TaskInfo == null) return;
            IoC.Get<IUIManager>().ShowPrompt(new PromptDialogBoxViewModel(async (input) =>
            {
                var result = true;
                try
                {
                    result = await IoC.Get<ITaskService>().PendingATask(TaskInfo.TaskId, input);
                    //refresh task
                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
                }
                catch (Exception exception)
                {
                    await IoC.Get<IUIManager>()
                        .ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "Error！",
                            Message = exception.Message
                        });
                }
                return result;
            })
            {
                Message = $"Pending task {TaskInfo.Content.GetFirstLineOrWords(50)}:",
                PromptMessage = "(optional)pending reason of this task"
            });
        }

        private void Failing()
        {
            IsMoreOpDisplayed = false;
            if (TaskInfo == null) return;
            IoC.Get<IUIManager>().ShowPrompt(new PromptDialogBoxViewModel(async (input) =>
            {
                var result = true;
                try
                {
                    result = await IoC.Get<ITaskService>().FailATask(TaskInfo.TaskId, input);
                    //refresh task
                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
                }
                catch (Exception exception)
                {
                    await IoC.Get<IUIManager>()
                        .ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "Error！",
                            Message = exception.Message
                        });
                }
                return result;
            })
            {
                Message = $"Fail task {TaskInfo.Content.GetFirstLineOrWords(50)}:",
                PromptMessage = "(optional)fail reason of this task"
            });
        }

        private void Delete()
        {
            IsMoreOpDisplayed = false;
            IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(TaskOperatorHelper.DeleteTask(false, TaskInfo))
            {
                CofirmText = "Confirm",
                CancelText = "Cancel",
                Message = "Are you sure to delete this child task?",
                SecondaryMessage = "You may restore it later.",
            });
        }

        #endregion
    }
}