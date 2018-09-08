using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;

namespace CN_Service
{
    public class MemoService : IMemoService
    {
        private readonly IMemoDataStore memoDataStore;
        private readonly ITaskDataStore taskDataStore;
        private readonly IUnitOfWork unitOfWork;

        public MemoService(IUnitOfWork UnitOfWork, IMemoDataStore MemoDataStore, ITaskDataStore TaskDataStore)
        {
            unitOfWork = UnitOfWork;
            memoDataStore = MemoDataStore;
            taskDataStore = TaskDataStore;
        }

        public async Task<string> AddAMemoToTask(CNMemo memo, int taskid)
        {
                if (memo == null) return string.Empty;
                var task = await taskDataStore.GetTask(taskid);
                if (task == null) return string.Empty;
                //Add memo To Task
                memo.TaskMemos.Add(new CNTaskMemo
                {
                    Task = task,
                    Memo = memo
                });
                //if MemoId is Null add the Memo
                if (string.IsNullOrEmpty(memo.MemoId))
                {
                    var memo1 = memo;
                    task.TaskTaggers.Select(x => x.Tag)
                        .ToList()
                        .ForEach(y => memo1.MemoTaggers.Add(new CNMemoTagger {Memo = memo1, Tag = y}));
                    memo = memoDataStore.AddMemo(memo);
                }
                //else Update it
                else
                {
                    memoDataStore.UpdateMemo(memo);
                }

                await unitOfWork.Commit();
                return memo?.MemoId;
        }

        public async Task<string> AddAMemoToGlobal(CNMemo memo)
        {
                if (memo == null) return string.Empty;
                memo = memoDataStore.AddMemo(memo);
                return await unitOfWork.Commit() ? memo?.MemoId : null;
        }

        public async Task<bool> UpdateAMemo(CNMemo memo)
        {
                if (memo == null) return false;
                memoDataStore.UpdateMemo(memo);
                return await unitOfWork.Commit();
        }

        public async Task<bool> DeleteAMemo(string memoId)
        {
                var memo = await memoDataStore.GetMemoById(memoId);
                if (memo == null) return false;
                memoDataStore.DeleteMemo(memo);
                return await unitOfWork.Commit();
        }

        public async Task<bool> RemoveAMemoFromTask(string memoId, int taskId)
        {
                //Assert memo exist
                var memo = await memoDataStore.GetMemoById(memoId);
                if (memo == null) return false;
                //Assert memo-task relation exist
                var tobeRemovedRelations = memo.TaskMemos.Where(x => x.TaskId == taskId).ToList();
                if (!tobeRemovedRelations.Any()) return false;
                //remove and update
                tobeRemovedRelations.ToList().ForEach(x => memo.TaskMemos.Remove(x));
                memoDataStore.UpdateMemo(memo);
                return await unitOfWork.Commit();
        }

        public async Task<ICollection<CNMemo>> GetAllGlobalMemos()
        {
                return await memoDataStore.GetAllGlobalMemos();
        }

        public async Task<ICollection<CNMemo>> GetAllTaskMemos(int taskid)
        {
                return await memoDataStore.GetAllTaskMemos(taskid);
        }

        public async Task<CNMemo> GetMemoById(string memoId)
        {
                return await memoDataStore.GetMemoById(memoId);
        }
    }
}