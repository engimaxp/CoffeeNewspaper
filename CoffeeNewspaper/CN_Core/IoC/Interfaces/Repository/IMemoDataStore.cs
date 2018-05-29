using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.IoC.Interfaces.Repository
{
    public interface IMemoDataStore
    {
        #region Update Methods

        /// <summary>
        ///     Update a Memo
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task<bool> UpdateMemo(CNMemo memo);

        #endregion

        #region Delete Methods

        /// <summary>
        ///     Delete a Memo
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task<bool> DeleteMemo(CNMemo memo);

        #endregion

        #region Select Methods

        /// <summary>
        ///     Select a memo by memoid
        /// </summary>
        /// <param name="memoid"></param>
        /// <returns></returns>
        Task<CNMemo> SelectMemoById(string memoid);

        /// <summary>
        ///     Get all memos with no task append to them
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNMemo>> GetAllGlobalMemos();

        /// <summary>
        ///     Get all memos append to specific task
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<ICollection<CNMemo>> GetAllTaskMemos(int taskid);

        #endregion

        #region Add Methods

        /// <summary>
        ///     Add a Memo
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task<CNMemo> AddMemo(CNMemo memo);

        /// <summary>
        ///     Deep copy of this memo
        /// </summary>
        /// <param name="originMemo">origin Memo being cloned</param>
        /// <returns>cloned Memo</returns>
        Task<CNMemo> CloneAMemo(CNMemo originMemo);

        #endregion
    }
}