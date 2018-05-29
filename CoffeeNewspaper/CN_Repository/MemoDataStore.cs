using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.IoC.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class MemoDataStore : BaseDataStore, IMemoDataStore
    {
        public MemoDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Select Methods

        public async Task<CNMemo> SelectMemoById(string memoid)
        {
            return await mDbContext.Memos.FirstOrDefaultAsync(r => string.Equals(r.MemoId, memoid));
        }

        public async Task<ICollection<CNMemo>> GetAllGlobalMemos()
        {
            return await mDbContext.Memos
                .Where(r=>r.TaskMemos.Count==0)
                .ToListAsync();
        }

        public async Task<ICollection<CNMemo>> GetAllTaskMemos(int taskid)
        {
            return await mDbContext.Memos
                .Where(r => r.TaskMemos.Any(y=>y.TaskId == taskid))
                .ToListAsync();
        }

        #endregion

        #region Delete Methods

        public async Task<bool> DeleteMemo(CNMemo memo)
        {
            mDbContext.Memos.Remove(memo);
            return await mDbContext.SaveChangesAsync() > 0;
        }

        #endregion

        #region Update Methods

        public async Task<bool> UpdateMemo(CNMemo memo)
        {
            mDbContext.Memos.Update(memo);
            return await mDbContext.SaveChangesAsync() >0;
        }

        #endregion

        #region Add Methods

        public async Task<CNMemo> AddMemo(CNMemo memo)
        {
            mDbContext.Memos.Add(memo);
            await mDbContext.SaveChangesAsync();
            return memo;
        }

        public async Task<CNMemo> CloneAMemo(CNMemo originMemo)
        {
            var newmemo = originMemo;
            newmemo.MemoId = null;
            mDbContext.Memos.Add(newmemo);
            await mDbContext.SaveChangesAsync();
            return newmemo;
        }

        #endregion
    }
}