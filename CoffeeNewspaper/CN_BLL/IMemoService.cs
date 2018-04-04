using System.Collections.Generic;
using CN_Model;

namespace CN_BLL
{
    public interface IMemoService
    {
        /// <summary>
        /// add memo to task
        /// </summary>
        /// <param name="memo"></param>
        /// <param name="taskid"></param>
        /// <returns>new memo's id</returns>
        string AddAMemoToTask(CNMemo memo,int taskid);
        /// <summary>
        /// add memo to global
        /// </summary>
        /// <param name="memo"></param>
        /// <returns>new memo's id</returns>
        string AddAMemoToGlobal(CNMemo memo);
        /// <summary>
        /// update memo both task and global
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        bool UpdateAMemo(CNMemo memo);

        /// <summary>
        /// delete a memo only in global or one task
        /// </summary>
        /// <param name="memoId"></param>
        /// <param name="taskId">if taskid equals zero than search in global memo</param>
        bool DeleteAMemo(string memoId,int taskId);

        List<CNMemo> GetAllGlobalMemos();

        List<CNMemo> GetAllTaskMemos(int taskid);

        CNMemo GetMemoById(string memoId);
    }
}