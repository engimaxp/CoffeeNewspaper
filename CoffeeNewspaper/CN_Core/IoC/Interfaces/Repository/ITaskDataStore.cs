using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core.Specification;

namespace CN_Core.Interfaces.Repository
{
    public interface ITaskDataStore
    {
        #region Delete Methods

        /// <summary>
        ///     remove a Task from datasource,physically removed forever
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        void RemoveTask(CNTask targetTask);

        /// <summary>
        ///  Remove a PreSufTaskConnector from datasource
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        void RemoveTaskConnector(CNTaskConnector connector);
        #endregion

        #region Add Methods

        /// <summary>
        ///     Add a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        CNTask AddTask(CNTask targetTask);

        #endregion

        #region Select Methods

        /// <summary>
        ///     Get all tasks from db
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNTask>> GetAllTask();

        /// <summary>
        ///     Get Task by id
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<CNTask> GetTask(int taskid);
        
        /// <summary>
        ///     Get Task by id (No tracking) used as clone object
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<CNTask> GetTaskNoTracking(int taskid);

        /// <summary>
        ///  Get all child tasks of a parent task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<ICollection<CNTask>> GetChildTasksNoTracking(int taskId);

        /// <summary>
        /// Get Max Sort
        /// </summary>
        /// <param name="parentTaskId"></param>
        /// <returns></returns>
        Task<int> GetMaxSort(int? parentTaskId);

        /// <summary>
        /// Get Tasks by specification
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task<ICollection<CNTask>> GetAllTasksBySpecification(ISpecification<CNTask> spec);
        #endregion

        #region Update Methods

        /// <summary>
        ///     update a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        void UpdateTask(CNTask targetTask);

        /// <summary>
        ///     Update EndTaskTime
        /// </summary>
        /// <param name="originDataTask">taskToBeUpdated</param>
        /// <param name="targetEndTime">EndTime</param>
        /// <returns></returns>
        void UpdateEndTaskTime(CNTask originDataTask, DateTime? targetEndTime);

        /// <summary>
        ///     Update StartTaskTime
        /// </summary>
        /// <param name="originDataTask">taskToBeUpdated</param>
        /// <param name="targetStartTime">StartTime</param>
        /// <returns></returns>
        void UpdateStartTaskTime(CNTask originDataTask, DateTime? targetStartTime);

        /// <summary>
        ///     ExpandTaskTime,Both StartTime and EndTime
        ///     if origin start-end interval is small or intersect new start-end interval
        ///     than update it
        /// </summary>
        /// <param name="originDataTask"></param>
        /// <param name="targetStartTime"></param>
        /// <param name="targetEndTime"></param>
        /// <returns></returns>
        void ExpandTaskTime(CNTask originDataTask, DateTime? targetStartTime, DateTime? targetEndTime);

        #endregion
    }
}