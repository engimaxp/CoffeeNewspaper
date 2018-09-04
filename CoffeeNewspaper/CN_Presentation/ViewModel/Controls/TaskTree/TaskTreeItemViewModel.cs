using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Core.Utilities;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel
{
    /// <summary>
    ///     Task Tree Item Model
    /// </summary>
    public class TaskTreeItemViewModel : BaseViewModel
    {
        #region Constructor

        public TaskTreeItemViewModel(ITreeNodeSubscribe Subscriber, CNTask taskinfo = null)
        {
            EditCommand = new RelayCommand(async ()=>await Edit());
            SelectCommand = new RelayCommand(Select);
            LeftCommand = new RelayCommand(async () => await Left());
            RightCommand = new RelayCommand(async () => await Right());
            UpCommand = new RelayCommand(async () => await Up());
            DownCommand = new RelayCommand(async ()=>await Down());

            this.Subscriber = Subscriber;

            TaskInfo = taskinfo ?? new CNTask();
            Title = TaskInfo.Content.GetFirstLineOrWords(50);
            Urgency = TaskInfo.MapFourQuadrantTaskUrgency();
            IsCompleted = TaskInfo.Status == CNTaskStatus.DONE;
            CurrentStatus = TaskInfo.MapTaskCurrentStatus();

            if (TaskInfo.HasParentTask())
            {
                DisplayLeftOperator = TaskInfo.ParentTask.HasParentTask();
                DisplayRightOperator =
                    TaskInfo.ParentTask.ChildTasks.FilterDeletedAndOrderBySortTasks().First().TaskId != TaskInfo.TaskId;
                DisplayUpOperator =
                    TaskInfo.ParentTask.ChildTasks.FilterDeletedAndOrderBySortTasks().First().TaskId != TaskInfo.TaskId;
                DisplayDownOperator =
                    TaskInfo.ParentTask.ChildTasks.FilterDeletedAndOrderBySortTasks().Last().TaskId != TaskInfo.TaskId;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Contianer Model
        /// </summary>
        private ITreeNodeSubscribe Subscriber { get; }

        #endregion

        #region Public Properties

        /// <summary>
        ///     TaskTitle
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Store the TaskInfo
        /// </summary>
        public CNTask TaskInfo { get; set; }

        /// <summary>
        ///     How urgent a task is
        /// </summary>
        public TaskUrgency Urgency { get; set; }

        /// <summary>
        ///     true if the treenode is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        ///     true if the task is completed
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        ///     Current Task Status
        /// </summary>
        public TaskCurrentStatus CurrentStatus { get; set; }

        /// <summary>
        ///     Child Tasks
        /// </summary>
        public ObservableCollection<TaskTreeItemViewModel> ChildItems { get; set; } =
            new ObservableCollection<TaskTreeItemViewModel>();

        public bool DisplayLeftOperator { get; set; }

        public bool DisplayRightOperator { get; set; }

        public bool DisplayUpOperator { get; set; }

        public bool DisplayDownOperator { get; set; }


        #endregion

        #region Commands

        public ICommand EditCommand { get; set; }

        public ICommand SelectCommand { get; set; }

        public ICommand LeftCommand { get; set; }

        public ICommand RightCommand { get; set; }

        public ICommand UpCommand { get; set; }

        public ICommand DownCommand { get; set; }

        #endregion

        #region Private Methods

        private void Select()
        {
            IsSelected ^= true;
            Subscriber?.SelectTargetNode(this);
        }

        private async Task Edit()
        {
            await IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Edit Task",
                FormContentViewModel = new TaskDetailFormViewModel(await IoC.Get<ITaskService>().GetTaskByIdNoTracking(TaskInfo.TaskId)),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }

        private async Task Down()
        {
            var taskinfo = await IoC.Get<ITaskService>().GetTaskById(TaskInfo.TaskId);

            //find next sibiling and exchange their sort
            var nextSibling = taskinfo?.ParentTask?.ChildTasks.FilterDeletedAndOrderBySortTasks()
                .FirstOrDefault(x => x.Sort > taskinfo.Sort);
            if (nextSibling == null) return;

            var temp = taskinfo.Sort;
            taskinfo.Sort = nextSibling.Sort;
            nextSibling.Sort = temp;

            await IoC.Get<ITaskService>().EditATask(taskinfo);
            await IoC.Get<ITaskService>().EditATask(nextSibling);

            //refresh view display
            await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(taskinfo.TaskId);
        }

        private async Task Up()
        {
            var taskinfo = await IoC.Get<ITaskService>().GetTaskById(TaskInfo.TaskId);

            //find prev sibiling and exchange their sort
            var prevSibling = taskinfo?.ParentTask?.ChildTasks.FilterDeletedAndOrderBySortTasks()
                .LastOrDefault(x => x.Sort < taskinfo.Sort);
            if (prevSibling == null) return;

            var temp = taskinfo.Sort;
            taskinfo.Sort = prevSibling.Sort;
            prevSibling.Sort = temp;

            await IoC.Get<ITaskService>().EditATask(taskinfo);
            await IoC.Get<ITaskService>().EditATask(prevSibling);

            //refresh view display
            await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(taskinfo.TaskId);
        }

        private async Task Right()
        {
            //find prev sibiling
            var taskinfo = await IoC.Get<ITaskService>().GetTaskById(TaskInfo.TaskId);

            var prevSibling = taskinfo?.ParentTask?.ChildTasks.FilterDeletedAndOrderBySortTasks()
                .LastOrDefault(x => x.Sort < taskinfo.Sort);
            if (prevSibling == null) return;
            //set current task's parent to sibiling
            await IoC.Get<ITaskService>().SetParentTask(taskinfo.TaskId, prevSibling.TaskId, -1);

            //refresh view display
            await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(taskinfo.TaskId);
        }

        private async Task Left()
        {
            //parent task will be a new prev sibiling
            //1.find new sibiling position of new parent's childrens
            //2.set current task's parent to new parent and set current task's position behind the new sibling

            var taskinfo = await IoC.Get<ITaskService>().GetTaskById(TaskInfo.TaskId);

            if (taskinfo.ParentTask?.ParentTask == null) return;
            var parentToBePrevSiblingPos = taskinfo.ParentTask.ParentTask.ChildTasks.FilterDeletedAndOrderBySortTasks()
                .ToList().FindIndex(x=>x.TaskId == taskinfo.ParentTask.TaskId);

            await IoC.Get<ITaskService>()
                .SetParentTask(taskinfo.TaskId, taskinfo.ParentTask.ParentTask.TaskId, parentToBePrevSiblingPos);

            //refresh view display
            await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(taskinfo.TaskId);
        }

        #endregion
    }
}