using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CN_Presentation.Input.Design;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Input;
using Microsoft.Recognizers.Text.DateTime;

namespace CN_Presentation.Input
{
    public class DateTimeEntryViewModel : HasChildSuggestButton<DateTimeSuggestButtonViewModel>,IUpdateDateTime
    {
        #region Private Properties

        private string _inputText;

        private IUpdateDateTime ParentInterface;

        private DateTime? _selectedDateTime;

        #endregion

        #region Public Methods

        public void SetParentInterface(IUpdateDateTime parentInterface)
        {
            ParentInterface = parentInterface;
        }
        #region Interface implement

        protected override Type GetTypeForValue()
        {
            return typeof(DateTime);
        }

        protected override IEnumerable<DateTimeSuggestButtonViewModel> CreateListUseModelLists(IEnumerable<SuggestDataDto> sdDtos)
        {
            return sdDtos?.Select(x =>
            {
                var temp = new DateTimeSuggestButtonViewModel()
                {
                    ParentModel = this,
                    Title = x.Title,
                };
                if (x.Value is DateTime time)
                {
                    temp.ValueDateTime = time;
                }
                return temp;
            });
        }
        public void NotifyUpdateDateTime(DateTime? time)
        {
            SelectedDateTime = time ?? DateTime.Now;
            IsPanelPopup = false;
        } 
        #endregion
        #endregion

        #region Constructor

        public DateTimeEntryViewModel()
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
        public DateTime? SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                if (_selectedDateTime == value)
                    return;

                _selectedDateTime = value;
                InputText = (_selectedDateTime ?? DateTime.Now).ToString("f");
                ParentInterface?.NotifyUpdateDateTime(_selectedDateTime);

                SuggestButtons = null;
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
                    ParentInterface?.NotifyUpdateDateTime(null);
                }
            }
        }

        /// <summary>
        /// Datetime Suggest Buttons
        /// if there is ambiguous recognitions
        /// create these buttons and show to the user to let them choose
        /// </summary>
        public ObservableCollection<DateTimeSuggestButtonViewModel> SuggestButtons { get; set; }

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

        /// <summary>
        /// the calendar controls viewModel
        /// </summary>
        public CalendarSelectControlViewModel PopUpCalendarSelectViewModel { get; set; } = new CalendarSelectControlDesignModel();

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
            if (IsPanelPopup)
            {
                var cscvm = new CalendarSelectControlViewModel();
                cscvm.SetUpdateTimeOfDayInterface(this);
                if (_selectedDateTime.HasValue)
                {
                    cscvm.CurrentlyValue = _selectedDateTime.Value;
                }
                PopUpCalendarSelectViewModel = cscvm;
            }
        }
        private void Clear()
        {
            if (!string.IsNullOrEmpty(InputText))
            {
                InputText = string.Empty;
                SuggestButtons = new ObservableCollection<DateTimeSuggestButtonViewModel>();
            }

            Editing = false;
        }

        private void Recognize()
        {
            if (string.IsNullOrEmpty(_inputText))
            {
                return;
            }
            var suggestResults = DateTimeRecognizer.RecognizeDateTime(_inputText, "zh-cn");
            var btnlist = CollectDateTimeSuggestButtonViewModels(suggestResults, this) ??
                          Enumerable.Empty<DateTimeSuggestButtonViewModel>();
            var list = btnlist.ToList();
            if (list.Count == 1)
            {
                var btn = list.First();
                SelectedDateTime = btn.ValueDateTime;
                InputText = btn.ValueDateTime.ToString("f");
                SuggestButtons = null;
            }
            else
            {
                SuggestButtons = new ObservableCollection<DateTimeSuggestButtonViewModel>(list);
            }

            RecgonizeFail = list.Count == 0;

             Editing = true;
            OnPropertyChanged(nameof(Editing));
        }

        #endregion

    }
}