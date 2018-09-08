using CN_Presentation.ViewModel.Form;
using CN_Presentation.ViewModel.Form.Design;

namespace CN_Presentation.ViewModel.Dialog.Design
{
    /// <summary>
    /// The design-time data for a <see cref="ConfirmDialogBoxViewModel"/>
    /// </summary>
    public class ConfirmDialogBoxDesignModel : ConfirmDialogBoxViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static ConfirmDialogBoxDesignModel Instance => new ConfirmDialogBoxDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConfirmDialogBoxDesignModel():base(null)
        {
            Message = "Are you sure you want to Delete the Task?";
            SecondaryMessage = "You may restore it later.";
            CofirmText = "Confirm";

            CancelText = "Cancel";
        }

        #endregion
    }
}