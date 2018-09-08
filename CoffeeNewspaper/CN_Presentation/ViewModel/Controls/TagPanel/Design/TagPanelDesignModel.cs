using System.Collections.ObjectModel;
using CN_Presentation.Input.Design;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class TagPanelDesignModel:TagPanelViewModel
    {

        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TagPanelDesignModel Instance => new TagPanelDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TagPanelDesignModel()
        {
            TagItems = new ObservableCollection<TagItemViewModel>()
            {
                new TagItemViewModel(this)
                {
                    TagTitle = "Works"
                },
                new TagItemViewModel(this)
                {
                    TagTitle = "ReUseableQuest"
                },
                new TagItemViewModel(this)
                {
                    TagTitle = "Garbage"
                },
            };
        }
        #endregion

    }
}