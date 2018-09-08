using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Input
{
    public class TagAddingControlViewModel : BaseViewModel
    {
        #region Private Properties

        private string _tagInput;

        private IAddNewTag ParentModel { get; set; }

        #endregion
        #region Constructor

        public TagAddingControlViewModel()
        {
            ClickAddCommand = new RelayCommand(ClickAdd);
            ConfirmAddCommand = new RelayCommand(ConfirmAdd);
            CancelAddCommand = new RelayCommand(CancelAdd);
            AutoCompleteBoxUpCommand = new RelayCommand(AutoCompleteBoxUp);
            AutoCompleteBoxDownCommand = new RelayCommand(AutoCompleteBoxDown);
        }
        #endregion
        #region Commands

        public ICommand ClickAddCommand { get; set; }

        public ICommand ConfirmAddCommand { get; set; }

        public ICommand CancelAddCommand { get; set; }

        public ICommand AutoCompleteBoxUpCommand { get; set; }

        public ICommand AutoCompleteBoxDownCommand { get; set; }

        public bool DisplayTextBox { get; set; }

        public string SelectedSearchAutoComplete { get; set; }

        public string TagInput
        {
            get => _tagInput;
            set
            {
                if (value == _tagInput) return;
                _tagInput = value;
                if (string.IsNullOrEmpty(_tagInput))
                {
                    TagRecommandCollection.Clear();
                    SelectedSearchAutoComplete = String.Empty;
                    IsAutoCompleteTagMenuOpened = false;
                }
                else
                {
                    Task.Run(async () =>
                    {
                        var recommendlist = await IoC.Get<ITagService>().GetStartStringTag(_tagInput);
                        TagRecommandCollection = new ObservableCollection<string>(recommendlist.Select(x=>x.Title).Except(ParentModel?.GetExistsTagsTitle()??new List<string>()));
                        IsAutoCompleteTagMenuOpened = TagRecommandCollection.Any();
                    });
                }
            }
        }


        public ObservableCollection<string> TagRecommandCollection { get; set; } = new ObservableCollection<string>();

        public bool IsAutoCompleteTagMenuOpened { get; set; }

        public bool SelectAndFocus { get; set; }

        #endregion

        #region Public Methods

        public void SetParentAddNewTagModel(IAddNewTag addNewTagModel)
        {
            ParentModel = addNewTagModel;
        }
        #endregion
        #region Private Methods

        private void AutoCompleteBoxDown()
        {
            if (!IsAutoCompleteTagMenuOpened) return;
            if (!TagRecommandCollection.Any()) return;
            int index = 0;
            var seachoptions = TagRecommandCollection.ToList();

            if (!string.IsNullOrEmpty(SelectedSearchAutoComplete))
            {
                index = seachoptions.FindIndex(x => x == SelectedSearchAutoComplete);
                if (index != seachoptions.Count - 1)
                {
                    index++;
                }
            }
            SelectedSearchAutoComplete = seachoptions[index];
        }

        private void AutoCompleteBoxUp()
        {
            if (!IsAutoCompleteTagMenuOpened) return;
            if (!TagRecommandCollection.Any()) return;
            int index = TagRecommandCollection.Count - 1;
            var seachoptions = TagRecommandCollection.ToList();
            if (!string.IsNullOrEmpty(SelectedSearchAutoComplete))
            {
                index = seachoptions.FindIndex(x => x == SelectedSearchAutoComplete);
                if (index != 0)
                {
                    index--;
                }
            }
            SelectedSearchAutoComplete = seachoptions[index];
        }

        private void ConfirmAdd()
        {
            if (IsAutoCompleteTagMenuOpened && !string.IsNullOrEmpty(SelectedSearchAutoComplete))
            {
                TagInput = SelectedSearchAutoComplete;
                IsAutoCompleteTagMenuOpened = false;
                SelectedSearchAutoComplete = string.Empty;
                SelectAndFocus = true;

                TagRecommandCollection.Clear();
                SelectedSearchAutoComplete = string.Empty;
                IsAutoCompleteTagMenuOpened = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(TagInput))
                {
                    ParentModel?.NotifyAddNewTag(TagInput);
                }
                DisplayTextBox = false;
                IsAutoCompleteTagMenuOpened = false;
                SelectAndFocus = false;
                TagInput = string.Empty;
            }
        }

        private void CancelAdd()
        {
            DisplayTextBox = false;
            IsAutoCompleteTagMenuOpened = false;
            SelectAndFocus = false;
            TagInput = string.Empty;
        }

        private void ClickAdd()
        {
            DisplayTextBox = true;
            SelectAndFocus = true;
        } 
        #endregion
    }
}