using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class HeadMenuButtonViewModel:BaseViewModel
    {
        #region Constructor

        public HeadMenuButtonViewModel()
        {
            JumpCommand = new RelayParameterizedCommand(async (param) => { await JumpPage(param); });
        }

        /// <summary>
        /// Jump to another page
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private async Task JumpPage(object parameter)
        {
            var page = (ApplicationPage)Enum.Parse(typeof(ApplicationPage), parameter.ToString());
            IoC.Get<ApplicationViewModel>().GoToPage(page);
            await Task.Delay(1);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Jump to another page
        /// </summary>
        public ICommand JumpCommand { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// FontCode represent this Button
        /// </summary>
        public IconType FontCode { get; set; }

        /// <summary>
        /// The Page this btn represent to
        /// </summary>
        public ApplicationPage TargetPage { get; set; }

        /// <summary>
        /// true if its current page active display
        /// </summary>
        public bool IsHighLighted => (IoC.Get<ApplicationViewModel>().CurrentPage == TargetPage);

        #endregion

        public void InformPageChange()
        {
            OnPropertyChanged(nameof(IsHighLighted));
        }
    }
}
