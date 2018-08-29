using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Utilities;
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
                }, false);
        }

        public async Task<bool> RemoveTaskConnector(CNTaskConnector connector)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.TaskConnectors.Remove(connector);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, false);
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
                }, (CNTask) null);
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
                }, false);
        }

        public async Task UpdateEndTaskTime(CNTask originDataTask, DateTime? targetEndTime)
        {
            await IoC.Task.Run(
                async () =>
                {
                    if (originDataTask == null) return;
                    if (originDataTask.EndTime == null && targetEndTime == null) return;
                    if (originDataTask.EndTime != null && originDataTask.EndTime.Equals(targetEndTime)) return;
                    originDataTask.EndTime = targetEndTime;
                    mDbContext.Tasks.Update(originDataTask);
                    await mDbContext.SaveChangesAsync();
                });
        }

        public async Task UpdateStartTaskTime(CNTask originDataTask, DateTime? targetStartTime)
        {
            await IoC.Task.Run(
                async () =>
                {
                    if (originDataTask == null) return;
                    if (originDataTask.StartTime == null && targetStartTime == null) return;
                    if (originDataTask.StartTime != null && originDataTask.StartTime.Equals(targetStartTime)) return;
                    originDataTask.StartTime = targetStartTime;
                    mDbContext.Tasks.Update(originDataTask);
                    await mDbContext.SaveChangesAsync();
                });
        }

        public async Task ExpandTaskTime(CNTask originDataTask, DateTime? targetStartTime, DateTime? targetEndTime)
        {
            await IoC.Task.Run(
                async () =>
                {
                    //origintask null return
                    if (originDataTask == null) return;
                    //if this condition is fulfilled,than new time is smaller-or-equal compare to old ,do noting and return
                    if (originDataTask.StartTime != null && originDataTask.EndTime != null &&
                        !(originDataTask.StartTime > targetStartTime) && !(originDataTask.EndTime < targetEndTime) &&
                        targetEndTime != null) return;

                    //flag for update db
                    var needUpdate = false;
                    if (originDataTask.StartTime == null || originDataTask.StartTime > targetStartTime)
                    {
                        originDataTask.StartTime = targetStartTime;
                        needUpdate = true;
                    }

                    if (originDataTask.EndTime == null && targetEndTime != null || //Pause A Task
                        originDataTask.EndTime != null && targetEndTime == null || //Start A Task
                        originDataTask.EndTime != null && targetEndTime != null &&
                        originDataTask.EndTime < targetEndTime
                    ) //Update A Task timeslice
                    {
                        originDataTask.EndTime = targetEndTime;
                        needUpdate = true;
                    }

                    if (!needUpdate) return;
                    mDbContext.Tasks.Update(originDataTask);
                    await mDbContext.SaveChangesAsync();
                });
        }

        #endregion

        #region Select Methods

        public async Task<CNTask> GetTaskNoTracking(int taskid)
        {
            return await IoC.Task.Run(
                async () =>
                    await mDbContext.Tasks.Include(x=>x.TaskTaggers).ThenInclude(y=>y.Tag).AsNoTracking()
                        .FirstOrDefaultAsync(r => r.TaskId == taskid)
            );
        }

        public async Task<int> GetMaxSort(int? parentTaskId)
        {
            return await IoC.Task.Run(
                async () => 
                    await (from c in mDbContext.Tasks
                        where c.ParentTaskID == parentTaskId
                        select c.Sort).DefaultIfEmpty().MaxAsync()
            );
        }

        public async Task<ICollection<CNTask>> GetAllTask()
        {
            return await IoC.Task.Run(
                async () =>
                    await (from c in mDbContext.Tasks
                        where !c.IsDeleted
                        orderby c.Sort
                        select c).ToListAsync()
            );
        }

        public async Task<CNTask> GetTask(int taskid)
        {
            return await IoC.Task.Run(
                async () =>
                    await mDbContext.Tasks
                        .FirstOrDefaultAsync(r => r.TaskId == taskid)
            );
        }

        #endregion
    }
}