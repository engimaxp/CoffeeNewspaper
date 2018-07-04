using System.Collections.ObjectModel;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel:BaseViewModel
    {
        public ObservableCollection<TaskListItemViewModel> Items { get; set; }
        
        public ObservableCollection<string> ActivatedSearchTxts { get; set; }

        public ICommand FilterCommand { get; set; }

        public TaskListViewModel()
        {
            FilterCommand = new RelayCommand(Filter);
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