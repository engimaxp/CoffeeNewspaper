using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls.StatusBar
{
    public class StatusBarViewModel : BaseViewModel
    {
        #region Public Properties

        public bool IsResting { get; set; } = true;

        public TimeSpan CurrentTimeCounter { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// String Hour for <see cref="CurrentTimeCounter"/>
        /// </summary>
        public string Hour => (CurrentTimeCounter.Hours + CurrentTimeCounter.Days * 24).ToString().PadLeft(2, '0');
        
        /// <summary>
        /// String Minute for <see cref="CurrentTimeCounter"/>
        /// </summary>
        public string Minute => CurrentTimeCounter.Minutes.ToString().PadLeft(2, '0');

        /// <summary>
        /// String Second for <see cref="CurrentTimeCounter"/>
        /// </summary>
        public string Second => CurrentTimeCounter.Seconds.ToString().PadLeft(2, '0');

        public string CurrentTaskTitle { get; set; }

        public int CurrentTaskId { get; set; }

        public DateTime NowTime { get; set; } = DateTime.Now;

        /// <summary>
        /// String time for <see cref="NowTime"/>
        /// </summary>
        public string Time => NowTime.ToString("hh:mm:ss tt");

        /// <summary>
        /// String date for <see cref="NowTime"/>
        /// </summary>
        public string Date => NowTime.ToString("MMMM dd yyyy dddd");

        #endregion
        #region Public Methods

        public async Task ChangeToWork(string title,int taskId,TimeSpan alreadyTimeSpan)
        {
            await AsyncAwaiter.AwaitAsync(nameof(StatusBarViewModel), async () =>
            {
                await Task.Run(() =>
                {
                    if (!IsResting || title == CurrentTaskTitle) return;
                    CurrentTaskTitle = title;
                    IsResting = false;
                    if (CurrentTaskId != taskId)
                    {
                        CurrentTimeCounter = alreadyTimeSpan;
                        CurrentTaskId = taskId;
                    }
                });
            });
        }

        public async Task ChangeToRest(int taskId)
        {
            await AsyncAwaiter.AwaitAsync(nameof(StatusBarViewModel), async () =>
            {
                await Task.Run(() =>
                {
                    if (IsResting) return;
                    if (taskId == CurrentTaskId)
                    {
                        CurrentTaskTitle = string.Empty;
                        CurrentTaskId = -1;
                        IsResting = true;
                        CurrentTimeCounter = TimeSpan.Zero;
                    }
                });
            });
        }

        public async Task TimeSpanIncrement()
        {
            await AsyncAwaiter.AwaitAsync(nameof(StatusBarViewModel), async () =>
            {
                await Task.Run(() =>
                {
                    NowTime = DateTime.Now;
                    CurrentTimeCounter += TimeSpan.FromSeconds(1);
                });
            });
        }

        #endregion
    }
}