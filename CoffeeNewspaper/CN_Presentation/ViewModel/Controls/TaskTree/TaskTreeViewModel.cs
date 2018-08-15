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
            OnPropertyChanged(nameof(ItemSeleted));
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

        /// <summary>
        /// true if there is item in list
        /// </summary>
        public bool TreeHasItems => Items.Any();

        /// <summary>
        /// true if at least one Item is Selected
        /// </summary>
        public bool ItemSeleted => NodeItems.Any(x => x.IsSelected);

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
            //find selected item
            var selectedItem = NodeItems.FirstOrDefault(x => x.IsSelected);

            if (selectedItem?.TaskInfo != null)
            {
                IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(TaskOperatorHelper.DeleteTask(false, selectedItem.TaskInfo))
                {
                    CofirmText = "Confirm",
                    CancelText = "Cancel",
                    Message = "Are you sure to delete this child task?",
                    SecondaryMessage = "You may restore it later.",
                });
            }

        }

        #endregion
    }
}