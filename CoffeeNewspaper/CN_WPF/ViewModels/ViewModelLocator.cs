using CN_Core;
using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Controls;

namespace CN_WPF
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in Xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public Properties

        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel => IoC.Get<ApplicationViewModel>();

        /// <summary>
        /// The navigation bar view model
        /// </summary>
        public static HeadMenuViewModel HeadMenuViewModel => IoC.Get<HeadMenuViewModel>();
        #endregion
    }
}
