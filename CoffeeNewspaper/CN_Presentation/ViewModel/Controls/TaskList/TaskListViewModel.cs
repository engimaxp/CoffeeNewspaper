using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel : BaseViewModel
    {
        public TaskListViewModel()
        {
            FilterCommand = new RelayCommand(Filter);
            SortCommand = new RelayCommand(Sort);
        }

        public ObservableCollection<TaskListItemViewModel> Items { get; set; } = new ObservableCollection<TaskListItemViewModel>();

        public ObservableCollection<string> ActivatedSearchTxts { get; set; } = new ObservableCollection<string>();

        public ICommand FilterCommand { get; set; }

        public ICommand SortCommand { get; set; }

        public async Task RefreshTaskItems()
        {
            var tasks = await IoC.Get<ITaskService>().GetAllTasks();
            foreach (var cnTask in tasks)
            {
                Items.Add(new TaskListItemViewModel(cnTask)
                {
                    TaskTitle = cnTask.Content.GetFirstLineOrWords(50),
                    Urgency = cnTask.MapFourQuadrantTaskUrgency(),
                    Status = cnTask.MapTaskCurrentStatus(),
                });
            }
        }

        private void Sort()
        {
            IoC.Get<IUIManager>().ShowForm(new FormDialogViewModel
            {
                Title = "Add a Task",
                FormContentViewModel = new TaskDetailFormViewModel(),
                OKButtonText = "Confirm",
                CancelButtonText = "Cancel"
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