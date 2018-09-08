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

        /// <summary>
        ///     CloseEvent delegate declaration for event
        /// </summary>
        public delegate void CloseWindowHandler();
        
        /// <summary>
        ///     Dialog Window register its close window function to this event
        /// </summary>
        public event CloseWindowHandler CloseWindowEvent;

        /// <summary>
        ///     Raise Close Event for derived type to Close Dialog Window
        /// </summary>
        protected void RaiseCloseEvent()
        {
            CloseWindowEvent?.Invoke();
        }
    }
}