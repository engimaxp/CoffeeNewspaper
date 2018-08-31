using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls.StatusBar;
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
            ExpandTaskCommand = new RelayCommand(async ()=>await ExpandTask());
            OpenEditDialogCommand = new RelayCommand(async ()=>await OpenEditDialog());
            SelectTaskCommand = new RelayCommand(SelectTask);
            DisplayMoreOpCommand = new RelayCommand(DisplayMoreOp);
            PendingCommand = new RelayCommand(Pending);
            FailingCommand = new RelayCommand(Failing);
            DeleteCommand = new RelayCommand(Delete);
            StartCommand = new RelayCommand(async () => await Start());
            StopCommand = new RelayCommand(async () => await Stop());
            FinishCommand = new RelayCommand(async () => await Finish());

            this.TaskInfo = TaskInfo;
            if (TaskInfo != null) BindTaskToCurrentViewModel(TaskInfo);
        }

        #endregion

        #region Public Methods

        public async Task Refresh()
        {
            Refreshed = true;
            if (TaskInfo != null)
            {
                BindTaskToCurrentViewModel(TaskInfo);
                await RefreshExpanderView();
            }
        }

        public async Task RefreshExpanderView(int? childTaskId = null)
        {
            if (IsExpanded) await LoadDataToDetailExpanderView(childTaskId);
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
        ///     true if task is paused
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        ///     true if task is finished
        /// </summary>
        public bool IsFinished { get; set; }

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
        ///     Toggle display More Op
        /// </summary>
        public ICommand DisplayMoreOpCommand { get; set; }

        /// <summary>
        ///     Set task to pending
        /// </summary>
        public ICommand PendingCommand { get; set; }

        /// <summary>
        ///     Set task to Fail
        /// </summary>
        public ICommand FailingCommand { get; set; }

        /// <summary>
        ///     Delete target task
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        /// <summary>
        ///     Start task
        /// </summary>
        public ICommand StartCommand { get; set; }

        /// <summary>
        ///     Stop task
        /// </summary>
        public ICommand StopCommand { get; set; }

        /// <summary>
        ///     Finish task
        /// </summary>
        public ICommand FinishCommand { get; set; }

        #endregion

        #region Private Methods

        private void BindTaskToCurrentViewModel(CNTask taskInfo)
        {
            TaskTitle = taskInfo.Content.GetFirstLineOrWords(50);
            Urgency = taskInfo.MapFourQuadrantTaskUrgency();
            Status = taskInfo.MapTaskCurrentStatus();
            CanPending = taskInfo.Status != CNTaskStatus.DONE && !taskInfo.IsFail;
            CanFail = !taskInfo.IsFail;
            IsPaused = taskInfo.Status != CNTaskStatus.DOING;
            IsFinished = taskInfo.Status == CNTaskStatus.DONE;
            Task.Run(async () =>
            {
                if (IsPaused)
                {
                    await IoC.Get<StatusBarViewModel>().ChangeToRest(taskInfo.TaskId);
                }
                else
                {
                    await IoC.Get<StatusBarViewModel>().ChangeToWork(taskInfo.Content.GetFirstLineOrWords(20),
                        taskInfo.TaskId,
                        taskInfo.GetLastStartWorkTimeSpan());
                }
            });
        }

        private async Task ExpandTask()
        {
            if (!IsExpanded) await LoadDataToDetailExpanderView();
            IsExpanded ^= true;
        }

        private async Task OpenEditDialog()
        {
            await IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Edit Task",
                FormContentViewModel = new TaskDetailFormViewModel(TaskInfo),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }

        private void SelectTask()
        {
            IsSelected = true;
            //other task deselct
            foreach (var model in IoC.Get<TaskListViewModel>().Items
                .Where(x => x.TaskInfo?.TaskId != TaskInfo?.TaskId && x.IsSelected))
                model.IsSelected = false;
        }

        private void DisplayMoreOp()
        {
            IsMoreOpDisplayed ^= true;
            //other task deselct
            if (IsMoreOpDisplayed)
                foreach (var model in IoC.Get<TaskListViewModel>().Items
                    .Where(x => x.TaskInfo?.TaskId != TaskInfo?.TaskId && x.IsMoreOpDisplayed))
                    model.IsMoreOpDisplayed = false;
        }

        private async Task LoadDataToDetailExpanderView(int? childTaskId = null)
        {
            ExpandDetailViewModel = new TaskExpandDetailViewModel(TaskInfo, childTaskId);
        }

        private void Pending()
        {
            IsMoreOpDisplayed = false;
            if (TaskInfo == null) return;
            IoC.Get<IUIManager>().ShowPrompt(new PromptDialogBoxViewModel(async input =>
            {
                var result = true;
                await TaskOperatorHelper.WrapException(async () =>
                {
                    result = await IoC.Get<ITaskService>().PendingATask(TaskInfo.TaskId, input);
                    //refresh task
                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
                });
                return result;
            })
            {
                Message = $"Pending task {TaskInfo.Content.GetFirstLineOrWords(50)}:",
                PromptMessage = "(optional)pending reason of this task",
                TextInput = TaskInfo.PendingReason
            });
        }

        private void Failing()
        {
            IsMoreOpDisplayed = false;
            if (TaskInfo == null) return;
            IoC.Get<IUIManager>().ShowPrompt(new PromptDialogBoxViewModel(async input =>
            {
                var result = true;
                await TaskOperatorHelper.WrapException(async () =>
                {
                    result = await IoC.Get<ITaskService>().FailATask(TaskInfo.TaskId, input);
                    //refresh task
                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
                });
                return result;
            })
            {
                Message = $"Fail task {TaskInfo.Content.GetFirstLineOrWords(50)}:",
                PromptMessage = "(optional)fail reason of this task",
                TextInput = TaskInfo.FailReason
            });
        }

        private void Delete()
        {
            IsMoreOpDisplayed = false;
            IoC.Get<IUIManager>().ShowConfirm(
                new ConfirmDialogBoxViewModel(TaskOperatorHelper.DeleteTask(false, TaskInfo.TaskId))
                {
                    CofirmText = "Confirm",
                    CancelText = "Cancel",
                    Message = "Are you sure to delete this child task?",
                    SecondaryMessage = "You may restore it later."
                });
        }

        private async Task Finish()
        {
            await TaskOperatorHelper.WrapException(async () =>
            {
                IsMoreOpDisplayed = false;
                if (TaskInfo == null) return;
                await IoC.Get<ITaskService>().FinishATask(TaskInfo.TaskId);
                //refresh task
                await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
            });
        }

        private async Task Stop()
        {
            await TaskOperatorHelper.WrapException(async () =>
            {
                IsMoreOpDisplayed = false;
                if (TaskInfo == null) return;
                await IoC.Get<ITaskService>().PauseATask(TaskInfo.TaskId);
                IsPaused = true;
                //refresh task
                await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
            });
        }

        private async Task Start()
        {
            await TaskOperatorHelper.WrapException(async () =>
            {
                IsMoreOpDisplayed = false;
                if (TaskInfo == null) return;
                await IoC.Get<ITaskService>().StartATask(TaskInfo.TaskId);
                IsPaused = false;
                //refresh task
                await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(TaskInfo.TaskId);
            });
        }

        #endregion
    }
}