using System.Collections.ObjectModel;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel
{
    /// <summary>
    /// Task Tree Item Model
    /// </summary>
    public class TaskTreeItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// TaskTitle
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Store the TaskInfo
        /// </summary>
        public CNTask TaskInfo { get; set; }

        /// <summary>
        ///     How urgent a task is
        /// </summary>
        public TaskUrgency Urgency { get; set; }

        /// <summary>
        /// true if the treenode is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Contianer Model
        /// </summary>
        private ITreeNodeSubscribe Subscriber { get; set; }
        /// <summary>
        /// Child Tasks
        /// </summary>
        public ObservableCollection<TaskTreeItemViewModel> ChildItems { get; set; } = new ObservableCollection<TaskTreeItemViewModel>();
        #endregion

        public ICommand EditCommand { get; set; }

        public ICommand SelectCommand { get; set; }

        public TaskTreeItemViewModel(ITreeNodeSubscribe Subscriber,CNTask taskinfo = null)
        {
            EditCommand = new RelayCommand(Edit);
            SelectCommand = new RelayCommand(Select);
            this.Subscriber = Subscriber;
            this.TaskInfo = taskinfo??new CNTask();
            this.Title = TaskInfo.Content.GetFirstLineOrWords(50);
            this.Urgency = TaskInfo.MapFourQuadrantTaskUrgency();
        }

        private void Select()
        {
            IsSelected ^= true;
            Subscriber?.SelectTargetNode(this);
        }

        private void Edit()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Edit Task",
                FormContentViewModel = new TaskDetailFormViewModel(TaskInfo),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }
    }
}