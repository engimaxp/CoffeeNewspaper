using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using Microsoft.Recognizers.Text.DateTime;

namespace CN_Presentation.Input
{
    public class DateTimeEntryViewModel : BaseViewModel
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
        #endregion

        #region Constructor

        public DateTimeEntryViewModel()
        {
            RecognizeCommand = new RelayCommand(Recognize);
            ClearCommand = new RelayCommand(Clear);
        }

        #endregion

        #region Public Properties

        public DateTime? SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                if (_selectedDateTime == value)
                    return;

                _selectedDateTime = value;

                ParentInterface?.NotifyUpdateDateTime(_selectedDateTime);
            }
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText == value)
                    return;

                _inputText = value;

                if (string.IsNullOrEmpty(_inputText))
                    Recognize();
            }
        }

        public ObservableCollection<DateTimeSuggestButtonViewModel> SuggestButtons { get; set; }

        public bool DisplaySuggestButtons => SuggestButtons != null && SuggestButtons.Count > 0;

        public bool Editing { get; set; }

        public bool RecgonizeFail { get; set; }

        public string FailReason => RecgonizeFail ? "Can't recgonize" : string.Empty;
        #endregion

        #region Commands

        public ICommand RecognizeCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        #endregion

        #region Private Methods

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
            var btnlist = suggestResults.CollectDateTimeSuggestButtonViewModels(this) ??
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