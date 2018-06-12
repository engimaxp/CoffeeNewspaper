﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.Interfaces.Repository
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
        Task<CNMemo> GetMemoById(string memoid);

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
        ///     the origin Memo Must Exist in db
        /// </summary>
        /// <param name="memoid">origin Memo's id</param>
        /// <returns>cloned Memo</returns>
        Task<CNMemo> CloneAMemo(string memoid);

        #endregion
    }
}