using System.Threading.Tasks;
using System.Windows;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation;
using CN_Presentation.ViewModel.Controls;
using CN_Repository;
using CN_Service;

namespace CN_WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplicationSetupAsync();

            await IoC.Get<TaskListViewModel>().RefreshTaskItems();
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }
        /// <summary>
        /// Bind DI Container and other Important thing before window shows up
        /// </summary>
        /// <returns></returns>
        private void ApplicationSetupAsync()
        {
            IoC.Setup();
            IoC.Kernel.BindInitialViewModel();
            IoC.Kernel.BindCNDBContext();

            IoC.Kernel.BindSqliteDataStore();
            IoC.Kernel.BindServices();


            //Bind UI Manager So Could use be used in ViewModel
            IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());
        }
    }
 }
