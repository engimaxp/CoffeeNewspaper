using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Input
{
    public class TimeQuickSelectButtonViewModel:BaseViewModel
    {
        /// <summary>
        /// Generate 6*4 zero clock btns
        /// </summary>
        /// <param name="updateTimeOfDay"></param>
        /// <returns></returns>
        public static List<TimeQuickSelectButtonViewModel> GetAllBtns(IUpdateTimeOfDay updateTimeOfDay)
        {
            var result = new List<TimeQuickSelectButtonViewModel>();
            Enumerable.Range(0, 24).ToList().ForEach(x =>
            {
                var zeroBtn = new TimeQuickSelectButtonViewModel()
                {
                    Text = $"{x}:00",
                    RowNum = x/4,
                    ColNum = x%4
                };
                zeroBtn.SetUpdateTimeOfDayInterface(updateTimeOfDay);
                result.Add(zeroBtn);
            });
            return result;
        }

        private void SetUpdateTimeOfDayInterface(IUpdateTimeOfDay updateTimeOfDay)
        {
            UpdateTimeOfDay = updateTimeOfDay;
        }

        public int RowNum { get; set; }

        public int ColNum { get; set; }

        public string Text { get; set; }

        public DateTime TimeOfDay => Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + Text); 

        public IUpdateTimeOfDay UpdateTimeOfDay { get; set; }

        public ICommand ClickCommand { get; set; }
        
        public TimeQuickSelectButtonViewModel()
        {
            ClickCommand = new RelayCommand(Click);
        }

        private void Click()
        {
            UpdateTimeOfDay?.UpdateTimeOfDay(TimeOfDay);
        }
    }
}