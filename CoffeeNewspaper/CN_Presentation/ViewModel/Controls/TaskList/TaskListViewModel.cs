using System.Collections.Generic;
using System.Collections.ObjectModel;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel:BaseViewModel
    {
        public ObservableCollection<TaskListItemViewModel> Items { get; set; }
        
        public ObservableCollection<string> ActivatedSearchTxts { get; set; }
    }
}