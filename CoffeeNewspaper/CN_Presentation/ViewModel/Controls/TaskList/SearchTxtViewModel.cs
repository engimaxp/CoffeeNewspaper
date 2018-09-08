using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class SearchTxtViewModel:BaseViewModel
    {
        public string Text { get; set; }

        public ICommand DeleteSearchCommand { get; set; }

        private readonly INotifySearch _notifySearch;

        public SearchTxtViewModel(string text,INotifySearch notifySearch =null)
        {
            DeleteSearchCommand = new RelayCommand(DeleteSearch);
            _notifySearch = notifySearch;
            Text = text;
        }

        private void DeleteSearch()
        {
            _notifySearch?.DeleteSearch(Text);
        }
    }
}