using System.Collections.Generic;

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
            TagItems = new List<TagItemViewModel>()
            {
                new TagItemViewModel()
                {
                    TagTitle = "Works"
                },
                new TagItemViewModel()
                {
                    TagTitle = "ReUseableQuest"
                },
                new TagItemViewModel()
                {
                    TagTitle = "Garbage"
                },
            };
        }
        #endregion

    }
}