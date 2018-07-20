using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Input
{
    public class TagAddingControlViewModel : BaseViewModel
    {
        private string _tagInput;
        private string _selectedRecommandTag;

        public TagAddingControlViewModel()
        {
            ClickAddCommand = new RelayCommand(ClickAdd);
            ConfirmAddCommand = new RelayCommand(ConfirmAdd);
            CancelAddCommand = new RelayCommand(CancelAdd);
        }

        public ICommand ClickAddCommand { get; set; }

        public ICommand ConfirmAddCommand { get; set; }

        public ICommand CancelAddCommand { get; set; }

        public bool DisplayTextBox { get; set; }

        public bool PopupAutoTagMenul { get; set; }

        public bool SelectAndFocus { get; set; }

        private IAddNewTag ParentModel { get; set; }

        public void SetParentAddNewTagModel(IAddNewTag addNewTagModel)
        {
            ParentModel = addNewTagModel;
        }

        public string TagInput
        {
            get => _tagInput;
            set
            {
                SelectAndFocus = false;
                if (value == _tagInput) return;
                PopupAutoTagMenul = !string.IsNullOrEmpty(value) && !TagRecommandCollection.Contains(value);
                _tagInput = value;
                if (PopupAutoTagMenul)
                {
                    TagRecommandCollection = new ObservableCollection<string>()
                    {
                        "Test1",
                        "Test2",
                        "Test3",
                        "Test4",
                        "Test5",
                        "Test6",
                    };
                }
                else
                {
                    TagRecommandCollection = new ObservableCollection<string>();
                    SelectedRecommandTag = string.Empty;
                }
            }
        }

        public string SelectedRecommandTag
        {
            get => _selectedRecommandTag;
            set
            {
                _selectedRecommandTag = value;
                if (!string.IsNullOrEmpty(_selectedRecommandTag))
                {
                    TagInput = value;
                    PopupAutoTagMenul = false;
                    SelectAndFocus = true;
                }
            }
        }

        public ObservableCollection<string> TagRecommandCollection { get; set; } = new ObservableCollection<string>();
        
        private void ConfirmAdd()
        {
            if (!string.IsNullOrEmpty(TagInput))
            {
                ParentModel?.NotifyAddNewTag(TagInput);
            }
            DisplayTextBox = false;
            PopupAutoTagMenul = false;
            SelectAndFocus = false;
            TagInput = string.Empty;
        }

        private void CancelAdd()
        {
            DisplayTextBox = false;
            PopupAutoTagMenul = false;
            SelectAndFocus = false;
            TagInput = string.Empty;
        }

        private void ClickAdd()
        {
            DisplayTextBox = true;
            SelectAndFocus = true;
        }
    }
}