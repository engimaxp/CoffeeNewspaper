using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using CN_Presentation.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Input
{
    public class CalendarSelectControlViewModel : BaseViewModel, IUpdateTimeOfDay
    {
        #region Constructor

        public CalendarSelectControlViewModel()
        {
            GoToTodayCommand = new RelayCommand(GoToToday);
            HourCtlArrowUp = new RelayCommand(HourArrowUp);
            HourCtlArrowDown = new RelayCommand(HourArrowDown);
            MinuteCtlArrowUp = new RelayCommand(MinuteArrowUp);
            MinuteCtlArrowDown = new RelayCommand(MinuteArrowDown);
            TimeQSBtns = TimeQuickSelectButtonViewModel.GetAllBtns(this);
            ConfirmCommand = new RelayCommand(Cofirm);
        }


        #endregion

        #region Public Methods

        public void SetUpdateTimeOfDayInterface(IUpdateDateTime updateTimeOfDay)
        {
            ParentUpdateTimeOfDay = updateTimeOfDay;
        }
        #region Interface implement

        public void UpdateTimeOfDay(DateTime time)
        {
            SelectedTimeHour = time.TimeOfDay.Hours;
            SelectedTimeMinute = time.TimeOfDay.Minutes;
        }

        #endregion

        #endregion

        private IUpdateDateTime ParentUpdateTimeOfDay;

        #region Private Methods

        private void Cofirm()
        {
            ParentUpdateTimeOfDay?.NotifyUpdateDateTime(CurrentlyValue);
        }
        private void GoToToday()
        {
            DisplayDate = DateTime.Now.ToString("MM.dd.yyyy");
        }


        private void HourArrowUp()
        {
            if (SelectedTimeHour == 23)
            {
                SelectedTimeHour = 0;
            }
            else
            {
                SelectedTimeHour++;
            }
        }

        private void HourArrowDown()
        {
            if (SelectedTimeHour == 0)
            {
                SelectedTimeHour = 23;
            }
            else
            {
                SelectedTimeHour--;
            }
        }

        private void MinuteArrowUp()
        {
            if (SelectedTimeMinute == 59)
            {
                SelectedTimeMinute = 0;
            }
            else
            {
                SelectedTimeMinute++;
            }
        }

        private void MinuteArrowDown()
        {
            if (SelectedTimeMinute == 0)
            {
                SelectedTimeMinute = 59;
            }
            else
            {
                SelectedTimeMinute--;
            }
        }
        #endregion

        #region Commands

        /// <summary>
        ///     Click to let the calendar control display today's month view
        /// </summary>
        public ICommand GoToTodayCommand { get; set; }

        public ICommand HourCtlArrowUp { get; set; }

        public ICommand HourCtlArrowDown { get; set; }
        
        public ICommand MinuteCtlArrowUp { get; set; }

        public ICommand MinuteCtlArrowDown { get; set; }

        public ICommand ConfirmCommand { get; set; }
        #endregion

        #region Publiic Properties

        /// <summary>
        ///     The Date Calendar Control Display at,not selected
        /// </summary>
        public string DisplayDate { get; set; } = DateTime.Now.ToString("MM.dd.yyyy");

        /// <summary>
        ///     The Date Calendar Control Currently Selected
        /// </summary>
        public DateTime? SelectedDate { get; set; }

        /// <summary>
        ///     The Currently Value Of the Control
        ///     it should transfer to its parent
        /// </summary>
        public DateTime CurrentlyValue
        {
            get => (SelectedDate ?? DateTime.Now).Date
                .AddHours(SelectedTimeHour).AddMinutes(SelectedTimeMinute);
            set
            {
                SelectedDate = value.Date;
                SelectedTimeHour = value.Hour;
                SelectedTimeMinute = value.Minute;
            }
        }

        /// <summary>
        ///     The hour of time Currently input control suggest
        /// </summary>
        public int SelectedTimeHour { get; set; }

        /// <summary>
        ///     The minute of time Currently input control suggest
        /// </summary>
        public int SelectedTimeMinute { get; set; }
        
        /// <summary>
        ///     The display txt of hour input
        /// </summary>
        public string SelectedTimeHourTxt
        {
            get => SelectedTimeHour.ToString();
            set
            {
                if (int.TryParse(value, out var number) && number >= 0 && number <= 23) SelectedTimeHour = number;
            }
        }

        /// <summary>
        ///     The display txt of minute input
        /// </summary>
        public string SelectedTimeMinuteTxt
        {
            get => SelectedTimeMinute.ToString().PadLeft(2, '0');
            set
            {
                if (int.TryParse(value, out var number) && number >= 0 && number <= 59) SelectedTimeMinute = number;
            }
        }

        /// <summary>
        ///     Currently selected date for display
        /// </summary>
        public string Date => SelectedDate == null
            ? DateTime.Now.ToShortDateString()
            : SelectedDate.Value.ToShortDateString();

        /// <summary>
        ///     Quick select time Buttons
        /// </summary>
        public List<TimeQuickSelectButtonViewModel> TimeQSBtns { get; set; }

        #endregion
    }
}