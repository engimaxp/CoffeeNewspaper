using System.Windows.Input;
using CN_Core;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;

namespace CN_Presentation.ViewModel.Application
{
    public class WorkSpaceViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Side Task Menu Visible or Collapse
        /// </summary>
        public bool SideTaskMenuVisible { get; set; } = true;

        public TaskListViewModel TasksListViewModel { get; set; }= IoC.Get<TaskListViewModel>();
   

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
