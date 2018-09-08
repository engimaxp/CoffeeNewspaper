using System.Collections.Generic;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Controls
{
    public class HeadMenuViewModel : BaseViewModel
    {
        #region Public Properties

        public List<HeadMenuButtonViewModel> NavButtonItems { get; set; }
        
        #endregion

        public ICommand AddCommand { get; set; }

        public void InformPageChange()
        {
            this.NavButtonItems.ForEach(x => x.InformPageChange());
        }

        public HeadMenuViewModel()
        {
            AddCommand = new RelayCommand(AddTask);
        }
        private void AddTask()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Add a Task",
                FormContentViewModel = new TaskDetailFormViewModel(null),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
            });
        }
    }
}