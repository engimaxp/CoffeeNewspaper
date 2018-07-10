using System;
using System.Collections.Generic;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Input
{
    public class CalendarSelectControlViewModel:BaseViewModel,IUpdateTimeOfDay
    {
        public string DisplayDate { get; set; } = DateTime.Now.ToString("MM.dd.yyyy");
        
        public DateTime? SelectedDate { get; set; }

        public DateTime? SelectedTime { get; set; }

        public string Date => SelectedDate == null
            ? DateTime.Now.ToShortDateString()
            : SelectedDate.Value.ToShortDateString();

        public ICommand GoToTodayCommand { get; set; }

        public List<TimeQuickSelectButtonViewModel> TimeQSBtns { get; set; }

        public CalendarSelectControlViewModel()
        {
            GoToTodayCommand = new RelayCommand(GoToToday);
            TimeQSBtns = TimeQuickSelectButtonViewModel.GetAllBtns(this);
        }

        private void GoToToday()
        {
            DisplayDate = DateTime.Now.ToString("MM.dd.yyyy");
        }

        public void UpdateTimeOfDay(DateTime time)
        {
            if (SelectedTime == null) SelectedTime = time;
            else
            {
                SelectedTime = SelectedTime.Value.Date.Add(time.TimeOfDay);
            }
        }

    }
}