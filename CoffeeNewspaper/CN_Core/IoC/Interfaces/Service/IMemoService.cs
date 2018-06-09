using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.Interfaces.Service
{
    public interface IMemoService
    {
        /// <summary>
        ///     add memo to task
        ///     if it's already exist than just added it to the task
        ///     if it doesn't add a new memo and autotag the memo with task tags
        /// </summary>
        /// <param name="memo"></param>
        /// <param name="taskid"></param>
        /// <returns>new memo's id</returns>
        Task<string> AddAMemoToTask(CNMemo memo, int taskid);

        /// <summary>
        ///     add memo to global
        /// </summary>
        /// <param name="memo"></param>
        /// <returns>new memo's id</returns>
        Task<string> AddAMemoToGlobal(CNMemo memo);

        /// <summary>
        ///     update memo both task and global
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task<bool> UpdateAMemo(CNMemo memo);

        /// <summary>
        ///     delete a memo both in tasks and global
        /// </summary>
        /// <param name="memoId"></param>
        Task<bool> DeleteAMemo(string memoId);

        /// <summary>
        ///     remove a memo from task
        /// </summary>
        /// <param name="memoId"></param>
        /// <param name="taskId">if taskid equals zero than search in global memo</param>
        Task<bool> RemoveAMemoFromTask(string memoId, int taskId);

        /// <summary>
        ///  Get all Global Memos
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNMemo>> GetAllGlobalMemos();

        /// <summary>
        /// Get all task memos
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<ICollection<CNMemo>> GetAllTaskMemos(int taskid);

        /// <summary>
        /// Get Memo By MemoId
        /// </summary>
        /// <param name="memoId"></param>
        /// <returns></returns>
        Task<CNMemo> GetMemoById(string memoId);
    }
}