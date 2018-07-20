using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.Input.Design
{
    /// <summary>
    /// The design-time data for a <see cref="TagAddingControlViewModel"/>
    /// </summary>
    public class TagAddingControlDesignModel : TagAddingControlViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TagAddingControlDesignModel Instance => new TagAddingControlDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TagAddingControlDesignModel()
        {
        }

        #endregion
    }
}
