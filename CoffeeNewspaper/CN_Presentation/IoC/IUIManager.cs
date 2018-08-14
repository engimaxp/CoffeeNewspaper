using System.Threading.Tasks;
using CN_Presentation.ViewModel.Dialog;

namespace CN_Core.Interfaces
{
    public interface IUIManager
    {
        /// <summary>
        /// Create and display a form
        /// the content of the form is Decided by it's ViewModel
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Task ShowForm(FormDialogViewModel form);

        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowMessage(MessageBoxDialogViewModel viewModel);

        /// <summary>
        /// Displays a confirm message box to the user
        /// </summary>
        /// <returns></returns>
        Task ShowConfirm(ConfirmDialogBoxViewModel confirmViewModel);
    }
}