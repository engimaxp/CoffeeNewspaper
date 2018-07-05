using System.Collections.ObjectModel;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel:BaseViewModel
    {
        public ObservableCollection<TaskListItemViewModel> Items { get; set; }
        
        public ObservableCollection<string> ActivatedSearchTxts { get; set; }

        public ICommand FilterCommand { get; set; }

        public ICommand SortCommand { get; set; }

        public TaskListViewModel()
        {
            FilterCommand = new RelayCommand(Filter);
            SortCommand = new RelayCommand(Sort);
        }

        private void Sort()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel()
            {
                Title = "Add a Task",
                FormContentViewModel = new TaskDetailFormViewModel(),
                OKButtonText = "queding",
                CancelButtonText = "quxiao"
            });
        }

        private void Filter()
        {
            IoC.Get<IUIManager>().ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "Wrong password",
                Message = "The current password is invalid"
            });
        }
    }
}