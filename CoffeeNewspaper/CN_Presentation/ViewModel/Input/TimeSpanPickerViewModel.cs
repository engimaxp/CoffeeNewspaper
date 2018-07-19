using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CN_Presentation.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Input
{
    public class TimeSpanPickerViewModel:BaseViewModel
    {
        public void SetUpdateTimeRangeInterface(IUpdateTimeRange updateTimeRange)
        {
            UpdateTimeRange = updateTimeRange;
        }

        public IUpdateTimeRange UpdateTimeRange { get; set; }

        public ICommand ClearCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public List<long> Days { get; set; }

        public List<long> Hours { get; set; }

        public List<long> Minutes { get; set; }

        public long SelectDay { get; set; }
        public long SelectHour { get; set; }

        public long SelectMinute { get; set; }

        public long CurrentlyValue
        {
            get => ((SelectDay * 24 + SelectHour) * 60 + SelectMinute) * 60;
            set
            {
                var min = value / 60;
                var hour = min / 60;
                var day = hour / 24;
                SelectMinute = min - hour * 60;
                SelectHour = hour - day * 24;
                SelectDay = day > 31 ? 31 : day;
            }
        }

        public TimeSpanPickerViewModel()
        {
            ClearCommand = new RelayCommand(Clear);
            ConfirmCommand = new RelayCommand(Confirm);
            Days = Enumerable.Range(0, 32).Select(Convert.ToInt64).ToList();
            Hours = Enumerable.Range(0, 24).Select(Convert.ToInt64).ToList();
            Minutes = Enumerable.Range(0, 60).Select(Convert.ToInt64).ToList();
        }

        private void Confirm()
        {
            UpdateTimeRange.NotifyUpdateTimeRange(CurrentlyValue);
        }

        private void Clear()
        {
            SelectDay = 0;
            SelectHour = 0;
            SelectMinute = 0;
        }
    }
}