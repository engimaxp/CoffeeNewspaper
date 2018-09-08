using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Specification;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TaskDataStore : BaseDataStore, ITaskDataStore
    {
        public TaskDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Delete Methods

        public void RemoveTask(CNTask targetTask)
        {
            mDbContext.Tasks.Remove(targetTask);
        }

        public void RemoveTaskConnector(CNTaskConnector connector)
        {
            mDbContext.TaskConnectors.Remove(connector);
        }

        #endregion

        #region Add Methods

        public CNTask AddTask(CNTask targetTask)
        {
            mDbContext.Tasks.Add(targetTask);
            return targetTask;
        }

        #endregion

        #region Update Methods

        public void UpdateTask(CNTask targetTask)
        {
            var existingTask = mDbContext.Tasks
                .Include(b => b.TaskTaggers)
                .FirstOrDefault(b => b.TaskId == targetTask.TaskId);

            if (existingTask == null)
            {
                IoC.Logger.Log($"targetTask id {targetTask.TaskId} not exists");
            }
            else
            {
                mDbContext.Entry(existingTask).CurrentValues.SetValues(targetTask);
                foreach (var taskTagger in targetTask.TaskTaggers)
                {
                    var existingTaskTagger = existingTask.TaskTaggers
                        .FirstOrDefault(p => p.TaskTaggerId == taskTagger.TaskTaggerId);

                    if (existingTaskTagger == null)
                    {
                        existingTask.TaskTaggers.Add(taskTagger);
                    }
                    else
                    {
                        mDbContext.Entry(existingTaskTagger).CurrentValues.SetValues(taskTagger);
                    }
                }

                foreach (var taskTagger in existingTask.TaskTaggers)
                {
                    if (targetTask.TaskTaggers.All(p => p.TaskTaggerId != taskTagger.TaskTaggerId))
                    {
                        mDbContext.Remove(taskTagger);
                    }
                }
            }
        }

        public void UpdateEndTaskTime(CNTask originDataTask, DateTime? targetEndTime)
        {
            if (originDataTask == null) return;
            if (originDataTask.EndTime == null && targetEndTime == null) return;
            if (originDataTask.EndTime != null && originDataTask.EndTime.Equals(targetEndTime)) return;
            originDataTask.EndTime = targetEndTime;
            mDbContext.Tasks.Update(originDataTask);
        }

        public void UpdateStartTaskTime(CNTask originDataTask, DateTime? targetStartTime)
        {
            if (originDataTask == null) return;
            if (originDataTask.StartTime == null && targetStartTime == null) return;
            if (originDataTask.StartTime != null && originDataTask.StartTime.Equals(targetStartTime)) return;
            originDataTask.StartTime = targetStartTime;
            mDbContext.Tasks.Update(originDataTask);
        }

        public void ExpandTaskTime(CNTask originDataTask, DateTime? targetStartTime, DateTime? targetEndTime)
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
        }

        #endregion

        #region Select Methods

        public async Task<CNTask> GetTaskNoTracking(int taskid)
        {
            return await IoC.Task.Run(
                async () =>
                    await mDbContext.Tasks.Include(x=>x.ParentTask).Include(x=>x.TaskTaggers).ThenInclude(y=>y.Tag).AsNoTracking()
                        .FirstOrDefaultAsync(r => r.TaskId == taskid)
            );
        }

        public async Task<ICollection<CNTask>> GetChildTasksNoTracking(int taskId)
        {
            return await IoC.Task.Run(
                async () =>
                    await mDbContext.Tasks.Include(x => x.TaskTaggers).ThenInclude(y => y.Tag).AsNoTracking()
                        .Where(r => r.ParentTaskID == taskId).ToListAsync()
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

        public async Task<ICollection<CNTask>> GetAllTasksBySpecification(ISpecification<CNTask> spec)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    // fetch a Queryable that includes all expression-based includes
                    var queryableResultWithIncludes = spec.Includes
                        .Aggregate(mDbContext.Set<CNTask>().AsQueryable(),
                            (current, include) => current.Include(include));

                    // modify the IQueryable to include any string-based include statements
                    var secondaryResult = spec.IncludeStrings
                        .Aggregate(queryableResultWithIncludes,
                            (current, include) => current.Include(include));

                    // return the result of the query using the specification's criteria expression
                    return await secondaryResult
                        .Where(spec.Criteria).ToListAsync();
                });
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