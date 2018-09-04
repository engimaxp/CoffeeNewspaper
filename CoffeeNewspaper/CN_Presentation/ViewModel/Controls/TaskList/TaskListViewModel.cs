using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;
using CN_Presentation.ViewModel.Form;
using Ninject.Infrastructure.Language;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel : BaseViewModel,INotifySearch
    {
        #region Private Member

        private ObservableCollection<TaskListItemViewModel> _items = new ObservableCollection<TaskListItemViewModel>();

        private readonly Dictionary<string,Func<TaskListItemViewModel,string,bool>> searchFilters = new Dictionary<string,Func<TaskListItemViewModel,string, bool>>()
        {
            //Basic Filter
            {"*basic*", (item,str) => item?.TaskInfo?.Content != null}
        };

        private string _selectedSearchAutoComplete;
        private string _searchInput;

        #endregion
        #region Constructor

        public TaskListViewModel()
        {
            FilterCommand = new RelayCommand(Filter);
            SortCommand = new RelayCommand(Sort);
            SearchCommand = new RelayCommand(Search);
            MoreCommand = new RelayCommand(More);
            AutoCompleteBoxUpCommand = new RelayCommand(AutoCompleteBoxUp);
            AutoCompleteBoxDownCommand = new RelayCommand(AutoCompleteBoxDown);
        }

        #endregion

        #region Public Properties

        public bool IsSearchAutoCompletePanelPopup { get; set; }

        public ObservableCollection<string> SearchAutoCompleteOptions { get; set; } = new ObservableCollection<string>();

        public string SelectedSearchAutoComplete
        {
            get => _selectedSearchAutoComplete;
            set
            {
                _selectedSearchAutoComplete = value;
                if (!string.IsNullOrEmpty(SearchInput) && SearchInput.ToLower() != $"#{_selectedSearchAutoComplete}")
                {
                    SearchInput = $"#{_selectedSearchAutoComplete}";
                }
            }
        }

        public string SearchInput
        {
            get => _searchInput;
            set
            {
                _searchInput = value;
                if (!string.IsNullOrEmpty(_searchInput) && _searchInput.StartsWith("#"))
                {
                    //Async Set the popup panel
                    Task.Run(async () =>
                    {
                        var searchTag = _searchInput.TrimStart('#');
                        if (string.IsNullOrEmpty(searchTag)) return;
                        SearchAutoCompleteOptions = new ObservableCollection<string>((await IoC.Get<ITagService>().GetAllTaskTags())
                            .Where(x => x.Title.ToLower().Contains(searchTag))
                            .Select(z => z.Title));
                        IsSearchAutoCompletePanelPopup = SearchAutoCompleteOptions.Any();
                    });
                }
                else
                {
                    SearchAutoCompleteOptions.Clear();
                    IsSearchAutoCompletePanelPopup = false;
                }
            }
        }

        public ObservableCollection<TaskListItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                ApplyFilterAndSort();
            }
        }

        public ObservableCollection<TaskListItemViewModel> FilteredItems { get; set; } =
            new ObservableCollection<TaskListItemViewModel>();

        public ObservableCollection<SearchTxtViewModel> ActivatedSearchTxts { get; set; } = new ObservableCollection<SearchTxtViewModel>();

        #endregion

        #region Commands

        public ICommand FilterCommand { get; set; }

        public ICommand SortCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public ICommand MoreCommand { get; set; }

        public ICommand AutoCompleteBoxUpCommand { get; set; }

        public ICommand AutoCompleteBoxDownCommand { get; set; }

        #endregion

        #region Public Methods

        public void DeleteSearch(string searchtxt)
        {
            if (!searchFilters.ContainsKey(searchtxt)) return;
            searchFilters.Remove(searchtxt);

            ActivatedSearchTxts.RemoveAt(ActivatedSearchTxts.ToList().FindIndex(x=>x.Text == searchtxt));

            ApplyFilterAndSort();
        }

        public async Task RefreshTaskItems()
        {
            var tasks = (await IoC.Get<ITaskService>().GetAllTasks()).Where(x => !x.HasParentTask());

            if (Items.Count == 0)
            {
                foreach (var cnTask in tasks)
                    AddItem(new TaskListItemViewModel(cnTask));
            }
            else
            {
                foreach (var cnTask in tasks)
                {
                    var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == cnTask.TaskId);
                    if (index >= 0)
                    {
                        Items[index].TaskInfo = cnTask;
                        Items[index].Refresh();
                    }
                    else
                    {
                        AddItem(new TaskListItemViewModel(cnTask));
                    }
                }

                foreach (var tobeDeletedItem in Items.Where(x => !x.Refreshed)) Items.Remove(tobeDeletedItem);
            }
        }

        public async Task RefreshSpecificTaskItem(int taskId)
        {
            var task = await IoC.Get<ITaskService>().GetTaskById(taskId);
            //Parent Task
            if (task.HasParentTask())
                await RefreshChildTasks(taskId);
            else
                RefreshTopLevelTask(task);
        }

        #endregion

        #region Private Properties

        private void AddItem(TaskListItemViewModel itemViewModel)
        {
            Items.Add(itemViewModel);
            if (PassFilter(itemViewModel))
            {
                FilteredItems.Add(itemViewModel);
            }
        }

        private void RemoveItemAt(int index)
        {
            if (index >= Items.Count || index < 0) return;
            if (FilteredItems.Contains(Items[index]))
            {
                FilteredItems.Remove(Items[index]);
            }
            Items.RemoveAt(index);
        }

        private void ApplyFilterAndSort()
        {
            IEnumerable<TaskListItemViewModel> tempItems = Items;
            //Apply Search
            foreach (var searchFilter in searchFilters.Keys)
            {
                tempItems = tempItems.Where(x=>searchFilters[searchFilter](x,searchFilter));
            }
            FilteredItems =  new ObservableCollection<TaskListItemViewModel>(tempItems);
        }

        private bool PassFilter(TaskListItemViewModel itemViewModel)
        {
            //Apply Search
            foreach (var searchFilter in searchFilters.Keys)
            {
                if (!searchFilters[searchFilter](itemViewModel, searchFilter)) return false;
            }

            return true;
        }

        private void RefreshTopLevelTask(CNTask task)
        {
            if (task.IsDeleted)
            {
                var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == task.TaskId);
                if (index >= 0) RemoveItemAt(index);
            }
            else
            {
                if (Items.Count == 0)
                {
                    AddItem(new TaskListItemViewModel(task));
                }
                else
                {
                    var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == task.TaskId);
                    if (index >= 0)
                    {
                        Items[index].TaskInfo = task;
                        Items[index].Refresh();
                    }
                    else
                    {
                        AddItem(new TaskListItemViewModel(task));
                    }
                }
            }
        }

        private async Task RefreshChildTasks(int taskid)
        {
            //find root node
            var parentTaskId = await IoC.Get<ITaskService>().GetTaskRootParentId(taskid);

            //refresh its expander
            var index = Items.ToList().FindIndex(x => (x.TaskInfo?.TaskId ?? 0) == parentTaskId);
            if (index >= 0) Items[index].RefreshExpanderView(taskid);
        }

        private void Sort()
        {
            IoC.Get<IUIManager>().ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "Wrong password s",
                Message = "The current password is invalid"
            });
        }

        private void Filter()
        {
            IoC.Get<IUIManager>().ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "Wrong password f",
                Message = "The current password is invalid"
            });
        }

        private void Search()
        {

            if (string.IsNullOrEmpty(SearchInput)) return;
            var search = SearchInput.Trim().ToLower();

            if(searchFilters.ContainsKey(search)) return;

            if (search.StartsWith("#"))
            {
                searchFilters.Add(search, (item, str) =>
                {
                    var task = Task.Run(async () => await IoC.Get<ITagService>().GetAllTagByTaskIdAndTagTitle(item.TaskInfo.TaskId,str.TrimStart('#')));
                    task.Wait();
                    return task.Result.Any();
                });
            }
            else
            {
                searchFilters.Add(search, (item, str) => item.TaskInfo.Content.ToLower().Contains(str));
            }

            ActivatedSearchTxts.Add(new SearchTxtViewModel(search,this));
            ApplyFilterAndSort();

            SearchInput = string.Empty;
            IsSearchAutoCompletePanelPopup = false;
            SearchAutoCompleteOptions.Clear();
            SelectedSearchAutoComplete = string.Empty;
        }

        private void AutoCompleteBoxDown()
        {
            if (!IsSearchAutoCompletePanelPopup) return;
            if (!SearchAutoCompleteOptions.Any()) return;
            int index = 0;
            var seachoptions = SearchAutoCompleteOptions.ToList();

            if (!string.IsNullOrEmpty(_selectedSearchAutoComplete))
            {
                index = seachoptions.FindIndex(x => x == _selectedSearchAutoComplete);
                if (index != seachoptions.Count - 1)
                {
                    index++;
                }
            }
            SelectedSearchAutoComplete = seachoptions[index];
        }

        private void AutoCompleteBoxUp()
        {
            if (!IsSearchAutoCompletePanelPopup) return;
            if (!SearchAutoCompleteOptions.Any()) return;
            int index = SearchAutoCompleteOptions.Count-1;
            var seachoptions = SearchAutoCompleteOptions.ToList();
            if (!string.IsNullOrEmpty(_selectedSearchAutoComplete))
            {
                index = seachoptions.FindIndex(x => x == _selectedSearchAutoComplete);
                if (index != 0)
                {
                    index--;
                }
            }
            SelectedSearchAutoComplete = seachoptions[index];
        }

        private void More()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}