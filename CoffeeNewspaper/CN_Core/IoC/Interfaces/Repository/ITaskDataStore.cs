using System.Threading.Tasks;

namespace CN_Core.IoC.Interfaces.Repository
{
    public interface ITaskDataStore
    {
        /// <summary>
        ///     Add a memo to this task,if memo's id exists in this task ,then update it
        /// </summary>
        /// <param name="targetTask">the task tobe added</param>
        /// <param name="newMemo">a new memo</param>
        /// <returns>the current task instance</returns>
        Task<bool> AddOrUpdate(CNTask targetTask, CNMemo newMemo);

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
    }
}