using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Core.Utilities;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskListViewModel : BaseViewModel, INotifySearch
    {
        #region Constructor

        public TaskListViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            AutoCompleteBoxUpCommand = new RelayCommand(AutoCompleteBoxUp);
            AutoCompleteBoxDownCommand = new RelayCommand(AutoCompleteBoxDown);

            FilterSelectModel = CreateBubbleSelectViewModel();
            SortSelectModel = CreateSortSelectViewModel();
        }

        #endregion

        #region Private Member

        private ObservableCollection<TaskListItemViewModel> _items = new ObservableCollection<TaskListItemViewModel>();

        private readonly Dictionary<string, Func<TaskListItemViewModel, string, bool>> searchFilters =
            new Dictionary<string, Func<TaskListItemViewModel, string, bool>>
            {
                //Basic Filter
                {"*basic*", (item, str) => item?.TaskInfo?.Content != null}
            };

        private Func<CNTask, CNTask,int> sortSetting = (x, y) =>
        {
            if (x.TaskId > y.TaskId) return 1;
            else if (x.TaskId == y.TaskId) return 0;
            else return -1;
        };

        private string _selectedSearchAutoComplete;
        private string _searchInput;

        #endregion

        #region Public Properties

        public bool IsSearchAutoCompletePanelPopup { get; set; }

        public ObservableCollection<string> SearchAutoCompleteOptions { get; set; } =
            new ObservableCollection<string>();

        public string SelectedSearchAutoComplete
        {
            get => _selectedSearchAutoComplete;
            set
            {
                _selectedSearchAutoComplete = value;
                if (!string.IsNullOrEmpty(SearchInput) && SearchInput.ToLower() != $"#{_selectedSearchAutoComplete}")
                    SearchInput = $"#{_selectedSearchAutoComplete}";
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
                        SearchAutoCompleteOptions = new ObservableCollection<string>(
                            (await IoC.Get<ITagService>().GetAllTaskTags())
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

        public ObservableCollection<SearchTxtViewModel> ActivatedSearchTxts { get; set; } =
            new ObservableCollection<SearchTxtViewModel>();

        public BubbleSelectViewModel FilterSelectModel { get; set; }

        public BubbleSelectViewModel SortSelectModel { get; set; }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; set; }
        
        public ICommand AutoCompleteBoxUpCommand { get; set; }

        public ICommand AutoCompleteBoxDownCommand { get; set; }

        #endregion

        #region Public Methods

        public void DeleteSearch(string searchtxt)
        {
            if (!searchFilters.ContainsKey(searchtxt)) return;
            searchFilters.Remove(searchtxt);

            ActivatedSearchTxts.RemoveAt(ActivatedSearchTxts.ToList().FindIndex(x => x.Text == searchtxt));

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

        private Action<BubbleMenuButtonViewModel, Func<CNTask, CNTask, int>> BuildSortMenuButton()
        {
            return (btn, compareFunc) =>
            {
                ClearSelectOfBubbleMenu(SortSelectModel);
                SetIconAndColor(SortSelectModel, btn);
                sortSetting = compareFunc;
                ApplyFilterAndSort();
            };
        }

        private BubbleMenuButtonViewModel BubbleMenuButtonFactory(BubbleMenuButtonType type)
        {
            switch (type)
            {
                case BubbleMenuButtonType.SortDefault:
                    return new BubbleSortDefaultButton().GetButton(BuildSortMenuButton());
                case BubbleMenuButtonType.SortByCreateTime:
                    return new BubbleSortByCreateTimeButton().GetButton(BuildSortMenuButton());
                case BubbleMenuButtonType.SortByCreateTimeDesc:
                    return new BubbleSortCreateTimeReverseButton().GetButton(BuildSortMenuButton());
                case BubbleMenuButtonType.SortByRecently:
                    return new BubbleSortByRecentlyButton().GetButton(BuildSortMenuButton());
                case BubbleMenuButtonType.SortByUrgencyImportance:
                    return new BubbleSortByUrgencyImportanceButton().GetButton(BuildSortMenuButton());
                default: return null;
            }
        }

        private BubbleSelectViewModel CreateSortSelectViewModel()
        {
            return new BubbleSelectViewModel
            {
                DefaultIconFontFamily = FontFamilyType.FontAwesomeSolid,
                DefaultIconFontText = IconType.SortAmount,
                ToolTip = "Sort",
                BubbleMenuViewModel = new BubbleMenuViewModel
                {
                    Buttons = new ObservableCollection<BubbleMenuButtonViewModel>
                    {
                        BubbleMenuButtonFactory(BubbleMenuButtonType.SortDefault),
                        BubbleMenuButtonFactory(BubbleMenuButtonType.SortByCreateTime),
                        BubbleMenuButtonFactory(BubbleMenuButtonType.SortByCreateTimeDesc),
                        BubbleMenuButtonFactory(BubbleMenuButtonType.SortByRecently),
                        BubbleMenuButtonFactory(BubbleMenuButtonType.SortByUrgencyImportance)
                    }
                }
            };
        }

        private BubbleSelectViewModel CreateBubbleSelectViewModel()
        {
            return new BubbleSelectViewModel
            {
                DefaultIconFontFamily = FontFamilyType.FontAwesomeSolid,
                DefaultIconFontText = IconType.Filter,
                ToolTip = "Filter",
                BubbleMenuViewModel = new BubbleMenuViewModel
                {
                    Buttons = new ObservableCollection<BubbleMenuButtonViewModel>
                    {
                        new BubbleMenuButtonViewModel(FontFamilyType.FontAwesomeSolid,
                            IconType.ThLarge,
                            "Display All",
                            async btn =>
                        {
                            ClearSelectOfBubbleMenu(FilterSelectModel);
                            btn.IsSelected = true;
                            if (searchFilters.ContainsKey("*RemoveFail*"))
                            {
                                searchFilters.Remove("*RemoveFail*");
                            }

                            if (searchFilters.ContainsKey("*RemoveComplete*"))
                            {
                                searchFilters.Remove("*RemoveComplete*");
                            }

                            if (searchFilters.ContainsKey("*RemovePending*"))
                            {
                                searchFilters.Remove("*RemovePending*");
                            }

                            ApplyFilterAndSort();
                            await Task.Delay(1000);
                        })
                        {
                            IsSelected = true,
                        },
                        new BubbleMenuButtonViewModel(FontFamilyType.FontAwesomeSolid,
                            IconType.Times,
                            "Dont Display Fail",
                            async btn =>
                        {
                            ClearSelectOfTargetBubbleMenu(FilterSelectModel, "Display All");
                            btn.IsSelected ^= true;
                            if (btn.IsSelected)
                            {
                                searchFilters.Add("*RemoveFail*", (task, str) => !task.TaskInfo.IsFail);
                            }
                            else if (searchFilters.ContainsKey("*RemoveFail*"))
                            {
                                searchFilters.Remove("*RemoveFail*");
                            }
                            ApplyFilterAndSort();
                            await Task.Delay(1);
                        }),
                        new BubbleMenuButtonViewModel(
                            FontFamilyType.FontAwesomeSolid,
                            IconType.Check,
                            "Dont Display Complete",async btn =>
                        {
                            ClearSelectOfTargetBubbleMenu(FilterSelectModel, "Display All");
                            btn.IsSelected ^= true;
                            if (btn.IsSelected)
                            {
                                searchFilters.Add("*RemoveComplete*",
                                    (task, str) => task.TaskInfo.Status != CNTaskStatus.DONE);
                            }
                            else if (searchFilters.ContainsKey("*RemoveComplete*"))
                            {
                                searchFilters.Remove("*RemoveComplete*");
                            }

                            ApplyFilterAndSort();
                            await Task.Delay(1);
                        }),
                        new BubbleMenuButtonViewModel(
                            FontFamilyType.FontAwesomeSolid,
                            IconType.HourGlass,
                            "Dont Display Pending",async btn =>
                        {
                            ClearSelectOfTargetBubbleMenu(FilterSelectModel, "Display All");
                            btn.IsSelected ^= true;
                            if (btn.IsSelected)
                            {
                                searchFilters.Add("*RemovePending*",
                                    (task, str) => task.TaskInfo.Status != CNTaskStatus.PENDING);
                            }
                            else if (searchFilters.ContainsKey("*RemovePending*"))
                            {
                                searchFilters.Remove("*RemovePending*");
                            }

                            ApplyFilterAndSort();
                            await Task.Delay(1);

                        })
                    }
                }
            };
        }

        private void SetIconAndColor(BubbleSelectViewModel sortSelectModel, BubbleMenuButtonViewModel btn)
        {
            sortSelectModel.DefaultIconFontFamily = btn.IconFontFamily;
            sortSelectModel.DefaultIconFontText = btn.IconFontText;
        }

        private void ClearSelectOfBubbleMenu(BubbleSelectViewModel viewModel)
        {
            if (viewModel?.BubbleMenuViewModel == null || viewModel.BubbleMenuViewModel.Buttons == null) return;
            foreach (var bubbleMenuButtonViewModel in viewModel.BubbleMenuViewModel.Buttons)
            {
                bubbleMenuButtonViewModel.IsSelected = false;
            }
        }

        private void ClearSelectOfTargetBubbleMenu(BubbleSelectViewModel viewModel,string title)
        {
            if (viewModel?.BubbleMenuViewModel == null || viewModel.BubbleMenuViewModel.Buttons == null) return;
            foreach (var bubbleMenuButtonViewModel in viewModel.BubbleMenuViewModel.Buttons)
            {
                if (bubbleMenuButtonViewModel.ButtonText == title)
                {
                    bubbleMenuButtonViewModel.IsSelected = false;
                }
            }
        }

        private void AddItem(TaskListItemViewModel itemViewModel)
        {
            Items.Add(itemViewModel);
            if (PassFilter(itemViewModel)) FilteredItems.Add(itemViewModel);
        }

        private void RemoveItemAt(int index)
        {
            if (index >= Items.Count || index < 0) return;
            if (FilteredItems.Contains(Items[index])) FilteredItems.Remove(Items[index]);
            Items.RemoveAt(index);
        }

        private void ApplyFilterAndSort()
        {
            IEnumerable<TaskListItemViewModel> tempItems = Items;
            //Apply Search
            foreach (var searchFilter in searchFilters.Keys)
                tempItems = tempItems.Where(x => searchFilters[searchFilter](x, searchFilter))
                    .OrderBy(y=>y.TaskInfo, ComparerBuilder<CNTask>.Builder(sortSetting));
            FilteredItems = new ObservableCollection<TaskListItemViewModel>(tempItems);
        }

        private bool PassFilter(TaskListItemViewModel itemViewModel)
        {
            //Apply Search
            foreach (var searchFilter in searchFilters.Keys)
                if (!searchFilters[searchFilter](itemViewModel, searchFilter))
                    return false;

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
        

        private void Search()
        {
            if (string.IsNullOrEmpty(SearchInput)) return;
            var search = SearchInput.Trim().ToLower();

            if (searchFilters.ContainsKey(search)) return;

            if (search.StartsWith("#"))
                searchFilters.Add(search, (item, str) =>
                {
                    var task = Task.Run(async () =>
                        await IoC.Get<ITagService>()
                            .GetAllTagByTaskIdAndTagTitle(item.TaskInfo.TaskId, str.TrimStart('#')));
                    task.Wait();
                    return task.Result.Any();
                });
            else
                searchFilters.Add(search, (item, str) => item.TaskInfo.Content.ToLower().Contains(str));

            ActivatedSearchTxts.Add(new SearchTxtViewModel(search, this));
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
            var index = 0;
            var seachoptions = SearchAutoCompleteOptions.ToList();

            if (!string.IsNullOrEmpty(_selectedSearchAutoComplete))
            {
                index = seachoptions.FindIndex(x => x == _selectedSearchAutoComplete);
                if (index != seachoptions.Count - 1) index++;
            }

            SelectedSearchAutoComplete = seachoptions[index];
        }

        private void AutoCompleteBoxUp()
        {
            if (!IsSearchAutoCompletePanelPopup) return;
            if (!SearchAutoCompleteOptions.Any()) return;
            var index = SearchAutoCompleteOptions.Count - 1;
            var seachoptions = SearchAutoCompleteOptions.ToList();
            if (!string.IsNullOrEmpty(_selectedSearchAutoComplete))
            {
                index = seachoptions.FindIndex(x => x == _selectedSearchAutoComplete);
                if (index != 0) index--;
            }

            SelectedSearchAutoComplete = seachoptions[index];
        }
        

        #endregion
    }
}