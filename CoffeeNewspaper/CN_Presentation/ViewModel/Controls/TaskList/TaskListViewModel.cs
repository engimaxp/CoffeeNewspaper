using System.Collections.Generic;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel:BaseViewModel
    {
        public List<TaskListItemViewModel> Items { get; set; }
        
    }
}