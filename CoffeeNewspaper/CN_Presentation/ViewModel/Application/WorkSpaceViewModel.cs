using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Application
{
    public class WorkSpaceViewModel:BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// Side Task Menu Visible or Collapse
        /// </summary>
        public bool SideTaskMenuVisible { get; set; } = true;

        #endregion


        #region Constructor
        public WorkSpaceViewModel()
        {
            ToggleTaskMenuCommand = new RelayCommand(ToggleTaskMenu);
        }

        #endregion

        #region Commands

        public ICommand ToggleTaskMenuCommand { get; set; }
        #endregion

        #region Private Methods

        private void ToggleTaskMenu()
        {
            SideTaskMenuVisible ^= true;
        }
        #endregion
    }
}
