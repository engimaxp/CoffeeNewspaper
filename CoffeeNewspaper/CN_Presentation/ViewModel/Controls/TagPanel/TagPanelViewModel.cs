using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Presentation.Input.Design;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.ViewModel.Controls
{
    public class TagPanelViewModel : BaseViewModel, IAddNewTag
    {
        public TagPanelViewModel()
        {
            var tagAddingControlViewModel = TagAddingControlDesignModel.Instance;
            tagAddingControlViewModel.SetParentAddNewTagModel(this);
            AddTagEntry = tagAddingControlViewModel;
        }

        public ObservableCollection<TagItemViewModel> TagItems { get; set; } =
            new ObservableCollection<TagItemViewModel>();

        /// <summary>
        ///     Tagadding control view model
        /// </summary>
        public TagAddingControlViewModel AddTagEntry { get; set; }

        public async Task NotifyAddNewTag(string newTag)
        {
            if (string.IsNullOrEmpty(newTag)) return;
            if (TagItems.Any(x => x.TagTitle == newTag)) return;
            var existTag = await IoC.Get<ITagService>().GetTagByTitle(newTag);
            TagItems.Add(existTag != null
                ? new TagItemViewModel(this) {TagTitle = newTag, TagId = existTag.TagId}
                : new TagItemViewModel(this) {TagTitle = newTag});
        }

        public IEnumerable<string> GetExistsTagsTitle()
        {
            return TagItems.Select(x => x.TagTitle);
        }
    }

    public class TagItemViewModel : BaseViewModel
    {
        #region Constructor

        public TagItemViewModel(TagPanelViewModel parent)
        {
            Parent = parent;
            DeleteCommand = new RelayCommand(DeleteFromParent);
            JumpCommand = new RelayCommand(JumpToTagDetail);
        }

        #endregion

        #region Private Properties

        private TagPanelViewModel Parent { get; }

        #endregion

        #region Public Properties

        public string TagTitle { get; set; }

        public string TagId { get; set; }

        #endregion

        #region Private Methods

        private void JumpToTagDetail()
        {
            Debug.WriteLine($"Jump to {TagId} {TagTitle}");
        }

        private void DeleteFromParent()
        {
            Parent.TagItems.Remove(this);
        }

        #endregion

        #region Commands

        public ICommand DeleteCommand { get; set; }

        public ICommand JumpCommand { get; set; }

        #endregion
    }
}