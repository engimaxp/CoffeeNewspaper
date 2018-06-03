using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TaskDataStore : BaseDataStore, ITaskDataStore
    {
        public TaskDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Delete Methods

        public async Task<bool> RemoveTask(CNTask targetTask)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.Tasks.Remove(targetTask);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, exceptionDefaultResult: false);
        }

        #endregion

        #region Add Methods

        public async Task<CNTask> AddTask(CNTask targetTask)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.Tasks.Add(targetTask);
                    await mDbContext.SaveChangesAsync();
                    return targetTask;
                },(CNTask)null);
        }

        #endregion

        #region Update Methods

        public async Task<bool> UpdateTask(CNTask targetTask)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.Tasks.Update(targetTask);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, exceptionDefaultResult: false);
        }

        #endregion

        #region Select Methods

        public async Task<ICollection<CNTask>> GetAllTask()
        {
            return await IoC.Task.Run(
                async () =>
                    await mDbContext.Tasks.ToListAsync()
            );
        }

        public async Task<CNTask> GetTask(int taskid)
        {
            return await IoC.Task.Run(
                async () =>
                    await mDbContext.Tasks.FirstOrDefaultAsync(r => r.TaskId == taskid)
            );
        }

        #endregion
    }
}