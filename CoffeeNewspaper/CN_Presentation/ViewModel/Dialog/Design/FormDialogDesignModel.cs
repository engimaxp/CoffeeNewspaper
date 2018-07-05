using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Dialog.Design
{
    /// <summary>
    /// The design-time data for a <see cref="FormDialogViewModel"/>
    /// </summary>
    public class FormDialogDesignModel : FormDialogViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static FormDialogDesignModel Instance => new FormDialogDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FormDialogDesignModel()
        {
            Title = "Add a Task";
            FormContentViewModel = new TaskDetailFormViewModel();
            OKButtonText = "queding";
            CancelButtonText = "quxiao";
        }

        #endregion
    }
}