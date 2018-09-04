using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class MemoDataStore : BaseDataStore, IMemoDataStore
    {
        public MemoDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Delete Methods

        public void DeleteMemo(CNMemo memo)
        {
            mDbContext.Memos.Remove(memo);
        }

        #endregion

        #region Update Methods

        public void UpdateMemo(CNMemo memo)
        {
            mDbContext.Memos.Update(memo);
        }

        #endregion

        #region Select Methods

        public async Task<CNMemo> GetMemoById(string memoid)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    return await mDbContext.Memos
                        .Include(x=>x.MemoTaggers)
                        .Include(x=>x.TaskMemos)
                        .FirstOrDefaultAsync(r => string.Equals(r.MemoId, memoid));
                });
        }

        public async Task<ICollection<CNMemo>> GetAllGlobalMemos()
        {
            return await IoC.Task.Run(
                async () =>
                {
                    return await mDbContext.Memos
                        .Where(r => r.TaskMemos.Count == 0)
                        .ToListAsync();
                });
        }

        public async Task<ICollection<CNMemo>> GetAllTaskMemos(int taskid)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    return await mDbContext.Memos
                        .Where(r => r.TaskMemos.Any(y => y.TaskId == taskid))
                        .ToListAsync();
                });
        }

        #endregion

        #region Add Methods

        public CNMemo AddMemo(CNMemo memo)
        {
            mDbContext.Memos.Add(memo);
            return memo;
        }

        public CNMemo CloneAMemo(string memoid)
        {
            var originalEntity = mDbContext.Memos.AsNoTracking()
                .Include(r => r.MemoTaggers)
                .Include(x => x.TaskMemos)
                .FirstOrDefault(e => string.Equals(e.MemoId, memoid, StringComparison.Ordinal));
            if (originalEntity == null) return null;
            originalEntity.MemoId = null;
            foreach (var originalEntityMemoTagger in originalEntity.MemoTaggers)
            {
                originalEntityMemoTagger.MemoId = null;
                originalEntityMemoTagger.MemoTaggerId = null;
            }
            foreach (var originalEntityTaskMemo in originalEntity.TaskMemos)
            {
                originalEntityTaskMemo.MemoId = null;
                originalEntityTaskMemo.TaskMemoId = null;
            }
            mDbContext.Memos.Add(originalEntity);
            return originalEntity;
        }

        #endregion
    }
}