using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel
{
    /// <summary>
    ///     Task Tree Model
    /// </summary>
    public class TaskTreeViewModel : BaseViewModel, ITreeNodeSubscribe
    {
        #region Constructors

        public TaskTreeViewModel(CNTask taskinfo)
        {
            AddChildTaskCommand = new RelayCommand(AddChildTask);

            AddNextTaskCommand = new RelayCommand(AddNextTask);

            DeleteChildTaskCommand = new RelayCommand(DeleteChildTask);

            _taskinfo = taskinfo;
        }

        #endregion

        #region Public Methods

        #region Interface Implement

        public void SelectTargetNode(TaskTreeItemViewModel node)
        {
            foreach (var taskTreeItemViewModel in NodeItems.Where(x => x != node && x.IsSelected))
                taskTreeItemViewModel.IsSelected = false;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Tree Root Items
        /// </summary>
        public ObservableCollection<TaskTreeItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                _items.ToList().ForEach(x => NodeItems.AddRange(DFS(x)));
            }
        }

        public bool TreeHasItems => Items.Count > 0;

        #endregion

        #region Commands

        public ICommand AddNextTaskCommand { get; set; }

        public ICommand AddChildTaskCommand { get; set; }

        public ICommand DeleteChildTaskCommand { get; set; }

        #endregion

        #region Private Properties

        private ObservableCollection<TaskTreeItemViewModel> _items = new ObservableCollection<TaskTreeItemViewModel>();
        private CNTask _taskinfo { get; }

        /// <summary>
        ///     Tree Node Plain Items
        /// </summary>
        private List<TaskTreeItemViewModel> NodeItems { get; } = new List<TaskTreeItemViewModel>();

        #endregion

        #region Private Methods

        private IEnumerable<TaskTreeItemViewModel> DFS(TaskTreeItemViewModel a)
        {
            yield return a;
            foreach (var b in a.ChildItems)
            {
                foreach (var c in DFS(b))
                {
                    yield return c;
                }
            }
        }

        private CNTask GetSelectedItemParentTask()
        {
            //find selected item
            var selectedItem = NodeItems.FirstOrDefault(x => x.IsSelected);
            if (selectedItem != null) return selectedItem.TaskInfo.ParentTask;
            //if not find a selected item, add to root _taskinfo
            return _taskinfo;
        }

        private CNTask GetSelectedItemTask()
        {
            return NodeItems.FirstOrDefault(x => x.IsSelected)?.TaskInfo ?? _taskinfo;
        }

        private void AddChildTask()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Add Task",
                FormContentViewModel = new TaskDetailFormViewModel(null, GetSelectedItemTask()),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }

        private void AddNextTask()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Add Task",
                FormContentViewModel = new TaskDetailFormViewModel(null, GetSelectedItemParentTask()),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }

        private void DeleteChildTask()
        {
            IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(DeleteTask(false))
            {
                CofirmText = "Confirm",
                CancelText = "Cancel",
                Message = "Are you sure to delete this child task?",
                SecondaryMessage = "You may restore it later.",
            });
        }

        private Func<Task<bool>> DeleteTask(bool force)
        {
            return async () =>
            {
                var result = false;
                try
                {
                    //find selected item
                    var selectedItem = NodeItems.FirstOrDefault(x => x.IsSelected);

                    if (selectedItem?.TaskInfo != null)
                    {
                        result = await IoC.Get<ITaskService>().DeleteTask(selectedItem.TaskInfo.TaskId, force);
                        //refresh task
                        await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(selectedItem.TaskInfo.TaskId);
                    }
                }
                catch (TaskHasChildTasksException)
                {
                    await IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(DeleteTask(true))
                    {
                        CofirmText = "Confirm",
                        CancelText = "Cancel",
                        Message = "This task has child tasks",
                        SecondaryMessage = "Do you really want delete it along with its child tasks?",
                    });
                    result = true;
                }
                catch (TaskHasSufTasksException)
                {
                    await IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(DeleteTask(true))
                    {
                        CofirmText = "Confirm",
                        CancelText = "Cancel",
                        Message = "This task has suf tasks,",
                        SecondaryMessage = "Do you really want delete it along with its suf tasks?",
                    });
                    result = true;
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
            };
        }

        #endregion
    }
}