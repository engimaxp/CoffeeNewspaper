using System;
using System.Collections.Generic;
using System.Linq;
using CN_Model;
using CN_Repository;

namespace CN_BLL
{
    public class MemoService:IMemoService
    {
        private readonly IRootDataProvider rootDataProvider;

        public MemoService()
        {
            rootDataProvider = RootDataProvider.GetProvider();
        }
        public MemoService(IRootDataProvider rootDataProvider)
        {
            this.rootDataProvider = rootDataProvider;
        }

        public string AddAMemoToTask(CNMemo memo, int taskid)
        {
            if (memo == null) return string.Empty;
            memo.MemoId = Guid.NewGuid().ToString("N");
            var root = rootDataProvider.GetRootData();
            var task = root.GetTaskById(taskid);
            if (string.IsNullOrEmpty(task?.Content)) return string.Empty;
            task.AddOrUpdateMemo(memo);
            root.AddOrUpdateTask(task);
            rootDataProvider.Persistence(root);
            return memo.MemoId;
        }

        public string AddAMemoToGlobal(CNMemo memo)
        {
            if (memo == null) return string.Empty;
            memo.MemoId = Guid.NewGuid().ToString("N");
            var root = rootDataProvider.GetRootData();
            root.AddOrUpdateGlobalMemo(memo);
            rootDataProvider.Persistence(root);
            return memo.MemoId;
        }

        public bool UpdateAMemo(CNMemo memo)
        {
            if (memo == null) return false;
            var root = rootDataProvider.GetRootData();
            var originMemo = root.GetMemoById(memo.MemoId);
            if (string.IsNullOrEmpty(originMemo?.MemoId)) throw new ArgumentException("Memo does not exist");
            root.UpdateMemo(memo);
            rootDataProvider.Persistence(root);
            return true;
        }

        public bool DeleteAMemo(string memoId,int taskid)
        {
            bool result = false;
            var root = rootDataProvider.GetRootData();
            var originMemo = root.GetMemoById(memoId);
            if (string.IsNullOrEmpty(originMemo?.MemoId)) return false;
            if (taskid == 0)
            {
                result = root.MemoList.Remove(originMemo);
            }
            else
            {
                var task = root.GetTaskById(taskid);
                if (string.IsNullOrEmpty(task?.Content)) return false;
                result = task.Memos.Remove(originMemo);
            }
            if(result) rootDataProvider.Persistence(root);
            return result;
        }

        public List<CNMemo> GetAllGlobalMemos()
        {
            return rootDataProvider.GetRootData().GetAllUniqueMemo().ToList();
        }

        public List<CNMemo> GetAllTaskMemos(int taskid)
        {
            return rootDataProvider.GetRootData().GetTaskMemo(taskid).ToList();
        }

        public CNMemo GetMemoById(string memoId)
        {
            return rootDataProvider.GetRootData().GetMemoById(memoId);
        }
    }
}