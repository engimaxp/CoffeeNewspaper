using System.Threading.Tasks;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Dialog;

namespace CN_WPF
{
    public class UIManager:IUIManager
    {
        public Task ShowForm(FormDialogViewModel form)
        {
            return new FormDialogBox().ShowDialog(form);
        }

        public Task ShowMessage(MessageBoxDialogViewModel viewModel)
        {
            return new DialogMessageBox().ShowDialog(viewModel);
        }
    }
}