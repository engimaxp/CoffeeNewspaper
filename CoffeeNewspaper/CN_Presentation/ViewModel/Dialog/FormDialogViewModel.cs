using System.Threading.Tasks;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Dialog
{
    public class FormDialogViewModel:BaseDialogViewModel
    {
        /// <summary>
        /// The content of the form dialog
        /// </summary>
        public FormBaseViewModel FormContentViewModel { get; set; }

        /// <summary>
        /// OK Button Text
        /// </summary>
        public string OKButtonText { get; set; }

        /// <summary>
        /// Cancel Button Text
        /// </summary>
        public string CancelButtonText { get; set; }

        /// <summary>
        /// ConfirmCommand for View to use
        /// </summary>
        public ICommand ConfirmCommand { get; set; }

        public FormDialogViewModel()
        {
            ConfirmCommand = new RelayCommand(async () => await Confirm());
        }

        /// <summary>
        /// Confirm Async Action
        /// if the child form return success ,return true and use event to close window
        /// if false, than do noting ,let the child form handle the error ,like pop a messagebox etc.
        /// </summary>
        /// <returns></returns>
        private async Task Confirm()
        {
            if (FormContentViewModel != null && await FormContentViewModel.Confirm())
            {
                RaiseCloseEvent();
            }
        }
    }
}