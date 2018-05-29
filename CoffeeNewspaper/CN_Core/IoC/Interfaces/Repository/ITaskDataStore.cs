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
        Task<bool> AddTask(CNTask targetTask);

        /// <summary>
        ///     Add a memo to this task,if memo's id exists in this task ,then update it
        /// </summary>
        /// <param name="targetTask">the task tobe added</param>
        /// <param name="newMemo">a new memo</param>
        /// <returns>the current task instance</returns>
        Task<bool> AddMemoToTask(CNTask targetTask, CNMemo newMemo);

        #endregion

        #region Update Methods

        /// <summary>
        ///     update a Task to datasource
        /// </summary>
        /// <param name="targetTask"></param>
        /// <returns></returns>
        Task<bool> UpdateTask(CNTask targetTask);

        /// <summary>
        ///     Append this task to parent task
        /// </summary>
        /// <param name="targetTask">the child task</param>
        /// <param name="parentTask">the parent task</param>
        /// <returns></returns>
        Task<bool> SetParentTask(CNTask targetTask, CNTask parentTask);

        /// <summary>
        ///     replace all specified substring in task's Memos
        /// </summary>
        /// <param name="targetTask">the task tobe change Memos</param>
        /// <param name="originwords">the substring to be replaced</param>
        /// <param name="targetwords">the new substring</param>
        /// <returns></returns>
        Task<bool> ReplaceAWordOfATaskMemos(CNTask targetTask, string originwords, string targetwords);

        /// <summary>
        ///     Start a Task
        /// </summary>
        Task<bool> StartATask(CNTask targetTask);

        /// <summary>
        ///     Stop a Task
        /// </summary>
        Task<bool> StopATask(CNTask targetTask);

        /// <summary>
        ///     Finish a Task
        /// </summary>
        Task<bool> EndATask(CNTask targetTask);

        #endregion
    }
}