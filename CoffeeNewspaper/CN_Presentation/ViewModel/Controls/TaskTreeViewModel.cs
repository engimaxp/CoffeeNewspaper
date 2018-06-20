using System.Collections.ObjectModel;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel
{
    /// <summary>
    /// Task Tree Model
    /// </summary>
    public class TaskTreeViewModel:BaseViewModel
    {
        /// <summary>
        /// Tree Root Items
        /// </summary>
        public ObservableCollection<TaskTreeItemViewModel> Items { get; set; }
    }
}