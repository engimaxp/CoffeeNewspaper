using CN_Presentation.ViewModel.Form;
using CN_Presentation.ViewModel.Form.Design;

namespace CN_Presentation.ViewModel.Dialog.Design
{
    /// <summary>
    /// The design-time data for a <see cref="PromptDialogBoxViewModel"/>
    /// </summary>
    public class PromptDialogBoxDesignModel : PromptDialogBoxViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static PromptDialogBoxDesignModel Instance => new PromptDialogBoxDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PromptDialogBoxDesignModel():base(null)
        {
            Message = "Please input the fail reason:";
            PromptMessage = "(optional)Fail reason of this task";
        }

        #endregion
    }
}