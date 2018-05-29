using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.IoC.Interfaces.Repository
{
    public interface ITaskDataStore
    {
        #region Select Methods

        /// <summary>
        ///     Get all tasks from db
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNTask>> GetAllTask();

        /// <summary>
        /// Get Task by id
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<CNTask> GetTask(int taskid);
        #endregion

        #region Delete Methods

        /// <summary>
        ///     remove a Task from datasource,physically removed forever
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        Task<bool> RemoveTask(CNTask targetTask);

        #endregion

        #region Add Methods

        /// <summary>
        ///     Add a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        Task<CNTask> AddTask(CNTask targetTask);
        #endregion

        #region Update Methods

        /// <summary>
        ///     update a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        Task<bool> UpdateTask(CNTask targetTask);

        
        #endregion
    }
}