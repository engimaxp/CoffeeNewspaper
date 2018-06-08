using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between memo and task is many-to-one
    /// </summary>
    public class CNTaskMemo
    {
        #region Formatting Implement

        public override string ToString()
        {
            return $"{nameof(MemoId)}: {MemoId}, {nameof(TaskId)}: {TaskId}";
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string TaskMemoId { get; set; }

        public string MemoId { get; set; }
        public virtual CNMemo Memo { get; set; }
        public int TaskId { get; set; }
        public virtual CNTask Task { get; set; }

        #endregion
    }
}