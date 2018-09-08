using System;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.Input
{
    public class DateTimeSuggestButtonViewModel:BaseViewModel
    {
        public string Title { get; set; }

        public DateTime ValueDateTime { get; set; }

        public ICommand ClickCommand { get; set; }

        public DateTimeEntryViewModel ParentModel { get; set; }

        public DateTimeSuggestButtonViewModel()
        {
            ClickCommand = new RelayCommand(Click);
        }

        private void Click()
        {
            ParentModel.SelectedDateTime = ValueDateTime;
        }
    }
}