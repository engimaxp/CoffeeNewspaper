using CN_Core;
using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Controls.StatusBar;

namespace CN_WPF
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in Xaml files
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ApplicationViewModel = IoC.Get<ApplicationViewModel>();
        }
        #region Public Properties

        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel { get; set; }

        /// <summary>
        /// The navigation bar view model
        /// </summary>
        public static HeadMenuViewModel HeadMenuViewModel => IoC.Get<HeadMenuViewModel>();

        /// <summary>
        /// The Task list bar view model
        /// </summary>
        public static TaskListViewModel TaskListViewModel => IoC.Get<TaskListViewModel>();

        /// <summary>
        /// Status Bar Single View Model
        /// </summary>
        public static StatusBarViewModel StatusBarViewModel => IoC.Get<StatusBarViewModel>();

        #endregion
    }
}
