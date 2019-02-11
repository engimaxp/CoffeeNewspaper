using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls.Design;
using CN_Presentation.ViewModel.Dialog;

namespace CN_Presentation.ViewModel.Controls
{
    public class MemoListItemViewModel:BaseViewModel
    {
        private string _memoContent;
        private string _memoTitle;
        public CNMemo memo { private set; get; }
        public int currentTaskId { get; set; }
        public string MemoTitle
        {
            get => _memoTitle;
            set
            {
                _memoTitle = value;
                SaveChangeToDB((cnMemo)=>
                {
                    cnMemo.Title = _memoTitle;
                    return cnMemo;
                });
            }
        }

        private void SaveChangeToDB(Func<CNMemo, CNMemo> updateAction)
        {
            if (memo != null && !string.IsNullOrEmpty(memo.MemoId))
            {
                updateAction.Invoke(memo);
                IoC.Get<IMemoService>().UpdateAMemo(memo);
            }
            else
            {
                CNMemo newMemo = new CNMemo();
                updateAction.Invoke(newMemo);
                if (currentTaskId > 0)
                {
                    IoC.Get<IMemoService>().AddAMemoToTask(newMemo, currentTaskId);
                }
                else
                {
                    IoC.Get<IMemoService>().AddAMemoToGlobal(newMemo);
                }
                memo = newMemo;
            }
        }

        public string MemoContent
        {
            get => _memoContent;
            set
            {
                _memoContent = value;
                SaveChangeToDB((cnMemo) =>
                {
                    cnMemo.Content = _memoContent;
                    return cnMemo;
                });
            }
        }

        public TagPanelViewModel TagPanelViewModel { get; set; } = TagPanelDesignModel.Instance;

        public ICommand DeleteCommand { get; set; }

        public async Task Delete()
        {
            if (!string.IsNullOrEmpty(this.memo?.MemoId))
            {
                await IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(DeleteMemo(this.memo?.MemoId))
                {
                    CofirmText = "Confirm",
                    CancelText = "Cancel",
                    Message = "are you sure to DELETE this memo?",
                    SecondaryMessage = "you may recover this memo from db",
                });
            }
            else {
                await DeleteMemo(this.memo?.MemoId)();
            }
        }

        private Func<Task<bool>> DeleteMemo(string memoId)
        {
            return async () =>
            {
                bool result = false;
                if (string.IsNullOrEmpty(memoId)) result = true;
                else result = await IoC.Get<IMemoService>().DeleteAMemo(memoId);
                if (result)
                {
                    IoC.Get<MemoListControlViewModel>().NotifyRemoveMemo(memoId);
                }

                return result;
            };
        }

        public MemoListItemViewModel()
        {
            DeleteCommand = new RelayCommand(async()=>await Delete());
        }

        public MemoListItemViewModel(CNMemo memo,int? taskid = null):this()
        {
            this.memo = memo;
            if (memo != null)
            {
                MemoTitle = memo.Title;
                MemoContent = memo.Content;
            }

            this.currentTaskId = taskid ?? 0;
            
        }
    }
}