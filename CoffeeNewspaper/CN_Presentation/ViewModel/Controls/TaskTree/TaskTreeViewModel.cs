using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel
{
    /// <summary>
    /// Task Tree Model
    /// </summary>
    public class TaskTreeViewModel:BaseViewModel, ITreeNodeSubscribe
    {
        private ObservableCollection<TaskTreeItemViewModel> _items = new ObservableCollection<TaskTreeItemViewModel>();

        /// <summary>
        /// Tree Root Items
        /// </summary>
        public ObservableCollection<TaskTreeItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value; 
                _items.ToList().ForEach(x=>NodeItems.AddRange(DFS(x)));
            }
        }

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

        /// <summary>
        /// Tree Node Plain Items
        /// </summary>
        public List<TaskTreeItemViewModel> NodeItems { get; set; } = new List<TaskTreeItemViewModel>();

        public void SelectTargetNode(TaskTreeItemViewModel node)
        {
            foreach (var taskTreeItemViewModel in NodeItems.Where(x=>x!=node && x.IsSelected))
            {
                taskTreeItemViewModel.IsSelected = false;
            }
        }

        public bool TreeHasItems => Items.Count > 0;

        public ICommand AddNextTaskCommand { get; set; }
        public ICommand AddChildTaskCommand { get; set; }

        public TaskTreeViewModel(CNTask taskinfo)
        {
            AddChildTaskCommand = new RelayCommand(AddChildTask);

            AddNextTaskCommand = new RelayCommand(AddNextTask);

            _taskinfo = taskinfo;
        }

        private CNTask _taskinfo { get; set; }

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
            return (NodeItems.FirstOrDefault(x => x.IsSelected)?.TaskInfo)??_taskinfo;
        }
        private void AddChildTask()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Add Task",
                FormContentViewModel = new TaskDetailFormViewModel(null,GetSelectedItemTask()),
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
    }
}