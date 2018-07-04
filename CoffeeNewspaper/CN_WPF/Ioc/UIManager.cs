using System;
using System.Threading.Tasks;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Dialog;

namespace CN_WPF
{
    public class UIManager:IUIManager
    {
        public async Task ShowForm(FormDialogViewModel viewModel)
        {
            throw new NotImplementedException();
            //            return new FormWindowBox().ShowForm(viewModel);
        }

        public Task ShowMessage(MessageBoxDialogViewModel viewModel)
        {
            return new DialogMessageBox().ShowDialog(viewModel);
        }
    }
}