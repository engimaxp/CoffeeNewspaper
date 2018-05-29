using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.IoC.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TaskDataStore : ITaskDataStore
    {
        #region Protected Members

        /// <summary>
        ///     The database context for the client data store
        /// </summary>
        protected CNDbContext mDbContext;

        #endregion

        #region Constructor

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public TaskDataStore(CNDbContext dbContext)
        {
            // Set local member
            mDbContext = dbContext;
        }

        #endregion

        #region Select Methods

        public async Task<ICollection<CNTask>> GetAllTask()
        {
            return await mDbContext.Tasks.ToListAsync();
        }

        #endregion

        #region Delete Methods

        public async Task<bool> RemoveTask(CNTask targetTask)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        #endregion

        #region Add Methods

        public async Task<bool> AddTask(CNTask targetTask)
        {
            mDbContext.Tasks.Add(targetTask);
            return await mDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMemoToTask(CNTask targetTask, CNMemo newMemo)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        #endregion

        #region Update Methods

        public async Task<bool> UpdateTask(CNTask targetTask)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        public async Task<bool> SetParentTask(CNTask targetTask, CNTask parentTask)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        public async Task<bool> ReplaceAWordOfATaskMemos(CNTask targetTask, string originwords, string targetwords)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        public async Task<bool> StartATask(CNTask targetTask)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        public async Task<bool> StopATask(CNTask targetTask)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        public async Task<bool> EndATask(CNTask targetTask)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }

        #endregion
    }
}