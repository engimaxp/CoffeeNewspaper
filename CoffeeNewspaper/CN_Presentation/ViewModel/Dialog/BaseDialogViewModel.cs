using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Dialog
{
    public class BaseDialogViewModel : BaseViewModel
    {
        /// <summary>
        ///     Title of the Dialog window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Width of the Dialog window
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Height of the Dialog window
        /// </summary>
        public int Height { get; set; }
    }
}