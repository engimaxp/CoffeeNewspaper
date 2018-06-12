﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;

namespace CN_Service
{
    public class MemoService : IMemoService
    {
        private readonly IMemoDataStore memoDataStore;
        private readonly ITaskDataStore taskDataStore;

        public MemoService(IMemoDataStore MemoDataStore, ITaskDataStore TaskDataStore)
        {
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
                memo = await memoDataStore.AddMemo(memo);
            }
            //else Update it
            else
            {
                if (!await memoDataStore.UpdateMemo(memo)) return string.Empty;
            }
            
            return memo?.MemoId;
        }

        public async Task<string> AddAMemoToGlobal(CNMemo memo)
        {
            if (memo == null) return string.Empty;
            memo = await memoDataStore.AddMemo(memo);
            return memo?.MemoId;
        }

        public async Task<bool> UpdateAMemo(CNMemo memo)
        {
            if (memo == null) return false;
            return await memoDataStore.UpdateMemo(memo);
        }

        public async Task<bool> DeleteAMemo(string memoId)
        {
            var memo = await memoDataStore.GetMemoById(memoId);
            if (memo == null) return false;
            return await memoDataStore.DeleteMemo(memo);
        }

        public async Task<bool> RemoveAMemoFromTask(string memoId, int taskId)
        {
            //Assert memo exist
            var memo = await memoDataStore.GetMemoById(memoId);
            if (memo == null) return false;
            //Assert memo-task relation exist
            var tobeRemovedRelations = memo.TaskMemos.Where(x => x.TaskId == taskId).ToList();
            if(!tobeRemovedRelations.Any())return false;
            //remove and update
            tobeRemovedRelations.ToList().ForEach(x => memo.TaskMemos.Remove(x));
            return await memoDataStore.UpdateMemo(memo);
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