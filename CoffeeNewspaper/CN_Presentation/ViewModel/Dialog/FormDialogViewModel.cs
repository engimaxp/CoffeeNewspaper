using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Dialog
{
    public class FormDialogViewModel:BaseDialogViewModel
    {
        /// <summary>
        /// The content of the form dialog
        /// </summary>
        public BaseViewModel FormContentViewModel { get; set; }

        /// <summary>
        /// OK Button Text
        /// </summary>
        public string OKButtonText { get; set; }

        /// <summary>
        /// Cancel Button Text
        /// </summary>
        public string CancelButtonText { get; set; }
    }
}