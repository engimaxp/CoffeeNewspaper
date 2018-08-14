using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Dialog
{
    /// <summary>
    ///     Details for a confirm message box dialog
    /// </summary>
    public class ConfirmDialogBoxViewModel : BaseDialogViewModel
    {
        /// <summary>
        ///     this Task is invoked when the confirm button is clicked
        /// </summary>
        private readonly Func<Task<bool>> _confirmTask;

        public ConfirmDialogBoxViewModel(Func<Task<bool>> confirmTask)
        {
            Title = "Please Confirm";
            _confirmTask = confirmTask;
            ConfirmCommand = new RelayCommand(async () => await Confirm());
        }

        /// <summary>
        ///     The message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Secondary info about the confirm box (optional)
        /// </summary>
        public string SecondaryMessage { get; set; }

        /// <summary>
        /// Decide whether to display secondary message
        /// </summary>
        public bool DisplaySecondaryMessage => !string.IsNullOrEmpty(SecondaryMessage);

        /// <summary>
        ///     The text to use for the Confirm button
        /// </summary>
        public string CofirmText { get; set; } = "Confirm";

        /// <summary>
        ///     The text to use for the Cancel button
        /// </summary>
        public string CancelText { get; set; } = "Cancel";

        /// <summary>
        ///     Decide whether the confirm action is running
        /// </summary>
        public bool ConfirmIsRunning { get; set; }

        /// <summary>
        ///     ConfirmCommand for View to use
        /// </summary>
        public ICommand ConfirmCommand { get; set; }

        /// <summary>
        ///     Confirm Async Action
        ///     if the child form return success ,return true and use event to close window
        ///     if false, than do noting ,let the child form handle the error ,like pop a messagebox etc.
        /// </summary>
        /// <returns></returns>
        private async Task Confirm()
        {
            if (_confirmTask != null && await RunCommandAsyncGeneric(() => ConfirmIsRunning,  _confirmTask))
            {
                RaiseCloseEvent();
            }
        }
    }
}