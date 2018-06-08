using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     A Memo represent a work experience
    /// </summary>
    public class CNMemo
    {
        #region Interface Implementation

        #region Formatting Override

        public override string ToString()
        {
            return $"{nameof(MemoId)}: {MemoId}, {nameof(Title)}: {Title}";
        }

        #endregion

        #endregion

        #region Public Property

        /// <summary>
        ///     The Id of this Memo
        /// </summary>
        public string MemoId { get; set; }

        /// <summary>
        ///     Content of this Memo May be any format string like html or json
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Title of this Memo mostlikely it's plain text
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Tasks appended by this memo
        /// </summary>
        public ICollection<CNTaskMemo> TaskMemos
        {
            get => LazyLoader == null ? _taskMemos : LazyLoader?.Load(this, ref _taskMemos);
            set => _taskMemos = value;
        }

        /// <summary>
        ///     Tags of this Memo
        /// </summary>
        public ICollection<CNMemoTagger> MemoTaggers
        {
            get => LazyLoader == null ? _memoTaggers : LazyLoader?.Load(this, ref _memoTaggers);
            set => _memoTaggers = value;
        }

        #endregion

        #region LazyLoading

        private ICollection<CNTaskMemo> _taskMemos = new HashSet<CNTaskMemo>();

        private ICollection<CNMemoTagger> _memoTaggers = new HashSet<CNMemoTagger>();

        private CNMemo(Action<object, string> lazyLoader)
        {
            LazyLoader = lazyLoader;
        }


        private Action<object, string> LazyLoader { get; set; }

        #endregion
        #region Constructor

        public CNMemo()
        {
        } 
        #endregion
    }
}