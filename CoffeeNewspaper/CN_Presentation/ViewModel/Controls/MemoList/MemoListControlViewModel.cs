using System.Collections.ObjectModel;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class MemoListControlViewModel:BaseViewModel
    {
        public ObservableCollection<MemoListItemViewModel> Items { get; set; }

        public ObservableCollection<string> ActivatedSearchTxts { get; set; }
    }
}