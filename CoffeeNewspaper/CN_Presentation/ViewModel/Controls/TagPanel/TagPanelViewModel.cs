using System.Collections.ObjectModel;
using System.Windows.Input;
using CN_Presentation.Input.Design;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.ViewModel.Controls
{
    public class TagPanelViewModel : BaseViewModel,IAddNewTag
    {
        public ObservableCollection<TagItemViewModel> TagItems { get; set; } = new ObservableCollection<TagItemViewModel>();
        public void NotifyAddNewTag(string newTag)
        {
            if(!string.IsNullOrEmpty(newTag))
                TagItems.Add(new TagItemViewModel(this){TagTitle = newTag});
        }
        /// <summary>
        ///     Tagadding control view model
        /// </summary>
        public TagAddingControlViewModel AddTagEntry { get; set; }

        public TagPanelViewModel()
        {
            var tagAddingControlViewModel = TagAddingControlDesignModel.Instance;
            tagAddingControlViewModel.SetParentAddNewTagModel(this);
            AddTagEntry = tagAddingControlViewModel;
        }
    }

    public class TagItemViewModel : BaseViewModel
    {
        public string TagTitle { get; set; }

        private TagPanelViewModel Parent { get; set; }

        public TagItemViewModel(TagPanelViewModel parent)
        {
            Parent = parent;
            DeleteCommand = new RelayCommand(DeleteFromParent);
        }

        private void DeleteFromParent()
        {
            Parent.TagItems.Remove(this);
        }

        public ICommand DeleteCommand { get; set; }



    }
}