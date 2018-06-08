using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between memo and task is many-to-one
    /// </summary>
    public class CNTaskMemo
    {
        #region LazyLoading

        private CNMemo _memo;

        private CNTask _task;

        private CNTaskMemo(Action<object, string> lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        
        private Action<object, string> LazyLoader { get; set; }

        #endregion

        #region Constuctor

        public CNTaskMemo()
        {
            
        }
        #endregion

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
        public CNMemo Memo
        {
            get => LazyLoader == null ? _memo : LazyLoader?.Load(this, ref _memo);
            set => _memo = value;
        }
        public int TaskId { get; set; }
        public CNTask Task
        {
            get => LazyLoader == null ? _task : LazyLoader?.Load(this, ref _task);
            set => _task = value;
        }

        #endregion
    }
}