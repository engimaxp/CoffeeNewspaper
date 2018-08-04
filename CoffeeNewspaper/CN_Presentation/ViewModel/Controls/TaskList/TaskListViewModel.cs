using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel : BaseViewModel
    {
        public TaskListViewModel()
        {
            FilterCommand = new RelayCommand(Filter);
            SortCommand = new RelayCommand(Sort);
        }

        public ObservableCollection<TaskListItemViewModel> Items { get; set; } =
            new ObservableCollection<TaskListItemViewModel>();

        public ObservableCollection<string> ActivatedSearchTxts { get; set; } = new ObservableCollection<string>();

        public ICommand FilterCommand { get; set; }

        public ICommand SortCommand { get; set; }

        public async Task RefreshTaskItems()
        {
            IEnumerable<CNTask> tasks = (await IoC.Get<ITaskService>().GetAllTasks()).Where(x => !x.HasParentTask());
            
            if (Items.Count == 0)
            {
                foreach (var cnTask in tasks)
                    Items.Add(new TaskListItemViewModel(cnTask));
            }
            else
            {
                foreach (var cnTask in tasks)
                {
                    var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == cnTask.TaskId);
                    if (index >= 0)
                    {
                        Items[index].TaskInfo = cnTask;
                        Items[index].Refresh();
                    }
                    else
                    {
                        Items.Add(new TaskListItemViewModel(cnTask));
                    }
                }

                foreach (var tobeDeletedItem in Items.Where(x => !x.Refreshed)) Items.Remove(tobeDeletedItem);
            }
        }

        public async Task RefreshSpecificTaskItem(int taskId)
        {
            var task = await IoC.Get<ITaskService>().GetTaskById(taskId);
            //Parent Task
            if (task.HasParentTask())
            {
                //find root node
                var parentTask = task.ParentTask;
                while (parentTask.ParentTask != null)
                {
                    parentTask = parentTask.ParentTask;
                }
                //refresh its expander
                var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == taskId);
                if (index >= 0)
                {
                    Items[index].RefreshExpanderView();
                }
            }
            else
            {
                if (Items.Count == 0)
                {
                    Items.Add(new TaskListItemViewModel(task));
                }
                else
                {
                    var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == taskId);
                    if (index >= 0)
                    {
                        Items[index].TaskInfo = task;
                        Items[index].Refresh();
                    }
                    else
                    {
                        Items.Add(new TaskListItemViewModel(task));
                    }
                }
            }
        }

        private void Sort()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Add a Task",
                FormContentViewModel = new TaskDetailFormViewModel(null),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }

        private void Filter()
        {
            IoC.Get<IUIManager>().ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "Wrong password",
                Message = "The current password is invalid"
            });
        }
    }
}