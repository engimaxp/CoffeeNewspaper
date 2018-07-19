using System;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.Input
{
    public class TimeRangeSuggestButtonViewModel : BaseViewModel
    {
        public string Title { get; set; }

        public long TimeRangeSecondsCount { get; set; }

        public ICommand ClickCommand { get; set; }

        public TimeRangeEntryViewModel ParentModel { get; set; }

        public TimeRangeSuggestButtonViewModel()
        {
            ClickCommand = new RelayCommand(Click);
        }

        private void Click()
        {
            ParentModel.SelectedTimeDuration = TimeRangeSecondsCount;
        }

    }
}