using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<bool> RemoveTask(CNTask targetTask);

        /// <summary>
        ///  Remove a PreSufTaskConnector from datasource
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        Task<bool> RemoveTaskConnector(CNTaskConnector connector);
        #endregion

        #region Add Methods

        /// <summary>
        ///     Add a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        Task<CNTask> AddTask(CNTask targetTask);

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
        /// Get Max Sort
        /// </summary>
        /// <param name="parentTaskId"></param>
        /// <returns></returns>
        Task<int> GetMaxSort(int? parentTaskId);

        #endregion

        #region Update Methods

        /// <summary>
        ///     update a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        Task<bool> UpdateTask(CNTask targetTask);

        /// <summary>
        ///     Update EndTaskTime
        /// </summary>
        /// <param name="originDataTask">taskToBeUpdated</param>
        /// <param name="targetEndTime">EndTime</param>
        /// <returns></returns>
        Task UpdateEndTaskTime(CNTask originDataTask, DateTime? targetEndTime);

        /// <summary>
        ///     Update StartTaskTime
        /// </summary>
        /// <param name="originDataTask">taskToBeUpdated</param>
        /// <param name="targetStartTime">StartTime</param>
        /// <returns></returns>
        Task UpdateStartTaskTime(CNTask originDataTask, DateTime? targetStartTime);

        /// <summary>
        ///     ExpandTaskTime,Both StartTime and EndTime
        ///     if origin start-end interval is small or intersect new start-end interval
        ///     than update it
        /// </summary>
        /// <param name="originDataTask"></param>
        /// <param name="targetStartTime"></param>
        /// <param name="targetEndTime"></param>
        /// <returns></returns>
        Task ExpandTaskTime(CNTask originDataTask, DateTime? targetStartTime, DateTime? targetEndTime);

        #endregion

    }
}