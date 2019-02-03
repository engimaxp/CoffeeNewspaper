using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class MemoListControlViewModel : BaseViewModel
    {
        private int currentTaskId { get; set; }

        public ObservableCollection<MemoListItemViewModel> Items { get; set; }= new ObservableCollection<MemoListItemViewModel>();

        public ObservableCollection<string> ActivatedSearchTxts { get; set; }


        public ICommand AddAMemoCommand { get; set; }

        public MemoListControlViewModel()
        {
            AddAMemoCommand = new RelayCommand(AddAMemo);
        }

        /**
         * delete selected memo
         */
        public void NotifyRemoveMemo(string memoId)
        {
            if (string.IsNullOrEmpty(memoId))
            {
                Stack<int> removeIndicies = new Stack<int>();
                for (var index = 0; index < Items.Count; index++)
                {
                    var memoListItemViewModel = Items[index];
                    if (memoListItemViewModel.memo == null || string.IsNullOrEmpty(memoListItemViewModel.memo.MemoId))
                    {
                        removeIndicies.Push(index);
                    }
                }
                while (removeIndicies.Count>0)
                {
                    Items.RemoveAt(removeIndicies.Pop());
                }
            }
            else
            {
                for (var index = 0; index < Items.Count; index++)
                {
                    var memoListItemViewModel = Items[index];
                    if (memoListItemViewModel.memo != null && memoId.Equals(memoListItemViewModel.memo.MemoId))
                    {
                        Items.RemoveAt(index);
                        break;
                    }
                }
            }
        }

        private void AddAMemo()
        {
            Items.Insert(0,new MemoListItemViewModel(null, currentTaskId));
        }
        public async Task LoadTaskMemos(int taskId)
        {
            if (currentTaskId == taskId)
            {
                return;
            }

            ICollection<CNMemo> taskMemos = await IoC.Get<IMemoService>().GetAllTaskMemos(taskId);
            Items.Clear();
            foreach (var taskMemo in taskMemos)
            {
                Items.Add(new MemoListItemViewModel(taskMemo,taskId));
            }

            await LoadGlobalMemos();
            currentTaskId = taskId;
        }

        /**
         * Load all global memos
         */
        private async Task LoadGlobalMemos()
        {
            ICollection<CNMemo> memos = await IoC.Get<IMemoService>().GetAllGlobalMemos();
            foreach (var cnMemo in memos)
            {
                Items.Add(new MemoListItemViewModel(cnMemo));
            }
        }

        public async Task FirstLoad()
        {
            await LoadGlobalMemos();
        }
    }
}