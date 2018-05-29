using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.IoC.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TaskDataStore : BaseDataStore,ITaskDataStore
    {

        #region Select Methods

        public async Task<ICollection<CNTask>> GetAllTask()
        {
            return await mDbContext.Tasks.ToListAsync();
        }

        public async Task<CNTask> GetTask(int taskid)
        {
            return await mDbContext.Tasks.FirstOrDefaultAsync(r=>r.TaskId == taskid);
        }
        #endregion

        #region Delete Methods

        public async Task<bool> RemoveTask(CNTask targetTask)
        {
            mDbContext.Tasks.Remove(targetTask);
            return await mDbContext.SaveChangesAsync()>0;
        }

        #endregion

        #region Add Methods

        public async Task<CNTask> AddTask(CNTask targetTask)
        {
            mDbContext.Tasks.Add(targetTask);
            await mDbContext.SaveChangesAsync();
            return targetTask;
        }
        
        #endregion

        #region Update Methods

        public async Task<bool> UpdateTask(CNTask targetTask)
        {
            mDbContext.Tasks.Update(targetTask);
            return await mDbContext.SaveChangesAsync()>0;
        }
        
        #endregion

        public TaskDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }
    }
}