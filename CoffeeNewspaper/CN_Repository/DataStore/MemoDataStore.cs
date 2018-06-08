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

        public async Task<bool> DeleteMemo(CNMemo memo)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.Memos.Remove(memo);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, false);
        }

        #endregion

        #region Update Methods

        public async Task<bool> UpdateMemo(CNMemo memo)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.Memos.Update(memo);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, false);
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

        public async Task<CNMemo> AddMemo(CNMemo memo)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.Memos.Add(memo);
                    await mDbContext.SaveChangesAsync();
                    return memo;
                },(CNMemo)null);
        }

        public async Task<CNMemo> CloneAMemo(string memoid)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    var originalEntity = mDbContext.Memos.AsNoTracking()
                        .Include(r => r.MemoTaggers)
                        .Include(x => x.TaskMemos)
                        .FirstOrDefault(e => string.Equals(e.MemoId, memoid, StringComparison.Ordinal));
                    if (originalEntity != null)
                    {
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
                        await mDbContext.SaveChangesAsync();
                        return originalEntity;
                    }

                    return null;
                }, (CNMemo)null);
        }

        #endregion
    }
}