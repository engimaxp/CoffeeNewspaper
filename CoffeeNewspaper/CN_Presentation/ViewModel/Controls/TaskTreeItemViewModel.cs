using System.Collections.ObjectModel;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel
{
    /// <summary>
    /// Task Tree Item Model
    /// </summary>
    public class TaskTreeItemViewModel : BaseViewModel
    {
        /// <summary>
        /// TaskTitle
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Child Tasks
        /// </summary>
        public ObservableCollection<TaskTreeItemViewModel> ChildItems { get; set; }
    }
}