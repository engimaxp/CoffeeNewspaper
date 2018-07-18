using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CN_Core.Utilities;
using CN_Presentation.Input;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;

namespace CN_Presentation.ViewModel.Input
{
    public class TimeRangeEntryViewModel:HasChildSuggestButton<TimeRangeSuggestButtonViewModel>, IUpdateTimeRange
    {

        #region Private Properties

        private string _inputText;

        private IUpdateTimeRange ParentInterface;

        private int _selectedTimeDuration;

        #endregion

        #region Public Methods

        public void SetParentInterface(IUpdateTimeRange parentInterface)
        {
            ParentInterface = parentInterface;
        }
        #region Interface implement

        protected override Type GetTypeForValue()
        {
            return _selectedTimeDuration.GetType();
        }

        protected override IEnumerable<TimeRangeSuggestButtonViewModel> CreateListUseModelLists(IEnumerable<SuggestDataDto> sdDtos)
        {
            return sdDtos?.Select(x=>
            {
                var temp = new TimeRangeSuggestButtonViewModel()
                {
                    ParentModel = this,
                    Title = x.Title,
                };
                if (x.Value is int timerange)
                {
                    temp.TimeRangeSecondsCount = timerange;
                }
                return temp;
            });
        }

        public void NotifyUpdateTimeRange(int timeRangeSeconds)
        {
            SelectedTimeDuration = timeRangeSeconds;
            IsPanelPopup = false;
        }
        #endregion
        #endregion

        #region Constructor

        public TimeRangeEntryViewModel()
        {
            RecognizeCommand = new RelayCommand(Recognize);
            ClearCommand = new RelayCommand(Clear);
            PopUpCommand = new RelayCommand(PopUp);
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// The Current Selected DateTime
        /// </summary>
        public int SelectedTimeDuration
        {
            get => _selectedTimeDuration;
            set
            {
                if (_selectedTimeDuration == value)
                    return;

                _selectedTimeDuration = value;
                InputText = new TimeSpan(_selectedTimeDuration*1000).GetTimeSpanLeftInfo();
                SuggestButtons = null;
                Editing = true;
                ParentInterface?.NotifyUpdateTimeRange(_selectedTimeDuration);
            }
        }

        /// <summary>
        /// The Current Input TextBox
        /// </summary>
        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText == value)
                    return;

                _inputText = value;

                if (string.IsNullOrEmpty(_inputText))
                {
                    RecgonizeFail = false;
                    SuggestButtons = null;
                }
            }
        }

        /// <summary>
        /// Datetime Suggest Buttons
        /// if there is ambiguous recognitions
        /// create these buttons and show to the user to let them choose
        /// </summary>
        public ObservableCollection<TimeRangeSuggestButtonViewModel> SuggestButtons { get; set; }

        /// <summary>
        /// display SuggestButtons or not 
        /// </summary>
        public bool DisplaySuggestButtons => SuggestButtons != null && SuggestButtons.Count > 0;

        /// <summary>
        /// true if the textbox need to be in edit state
        /// have focus and selectall
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// true if the RecgonizeText Fail
        /// </summary>
        public bool RecgonizeFail { get; set; }

        /// <summary>
        /// Recgonize Fail Reason
        /// </summary>
        public string FailReason => RecgonizeFail ? "Can't recgonize" : string.Empty;

        /// <summary>
        /// true if the calendar select control display
        /// </summary>
        public bool IsPanelPopup { get; set; }

        #endregion

        #region Commands

        public ICommand RecognizeCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand PopUpCommand { get; set; }
        #endregion

        #region Private Methods

        private void PopUp()
        {
            IsPanelPopup ^= true;
        }
        private void Clear()
        {
            if (!string.IsNullOrEmpty(InputText))
            {
                InputText = string.Empty;
                SuggestButtons = new ObservableCollection<TimeRangeSuggestButtonViewModel>();
            }

            Editing = false;
        }

        private void Recognize()
        {
            if (string.IsNullOrEmpty(_inputText))
            {
                return;
            }
            List<ModelResult> suggestResults = DateTimeRecognizer.RecognizeDateTime(_inputText, "zh-cn");
            IEnumerable<TimeRangeSuggestButtonViewModel> btnlist = CollectDateTimeSuggestButtonViewModels(suggestResults,this) ??
                          Enumerable.Empty<TimeRangeSuggestButtonViewModel>();
            var list = btnlist.ToList();
            if (list.Count == 1)
            {
                var btn = list.First();
                SelectedTimeDuration = btn.TimeRangeSecondsCount;
                InputText = new TimeSpan(btn.TimeRangeSecondsCount * 1000).GetTimeSpanLeftInfo();
                SuggestButtons = null;
            }
            else
            {
                SuggestButtons = new ObservableCollection<TimeRangeSuggestButtonViewModel>(list);
            }

            RecgonizeFail = list.Count == 0;

            Editing = true;
            OnPropertyChanged(nameof(Editing));
        }

        #endregion

    }
}