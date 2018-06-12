using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class HeadMenuViewModel : BaseViewModel
    {
        #region Constructor

        public HeadMenuViewModel()
        {
            ChangePageCommand = new RelayParameterizedCommand(async (param) => { await ChangePage(param);});
        }

        private async Task ChangePage(object parameter)
        {
            var page = (ApplicationPage) Enum.Parse(typeof(ApplicationPage), parameter.ToString());
            IoC.Get<ApplicationViewModel>().GoToPage(page);
            await Task.Delay(1);
        }

        #endregion

        #region Commands

        public ICommand ChangePageCommand { get; set; }

        #endregion

        #region Public Properties

        public bool IsWorkSpaceSelected { get; set; }

        #endregion

    }
}