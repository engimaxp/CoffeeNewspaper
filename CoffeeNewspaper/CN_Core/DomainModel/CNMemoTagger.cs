using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between memo and tag is one-to-many
    /// </summary>
    public class CNMemoTagger
    {
        #region Public Properties

        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string MemoTaggerId { get; set; }

        public string TagId { get; set; }
        public CNTag Tag
        {
            get => LazyLoader == null ? _tag : LazyLoader?.Load(this, ref _tag);
            set => _tag = value;
        }
        public string MemoId { get; set; }
        public CNMemo Memo
        {
            get => LazyLoader == null ? _memo : LazyLoader?.Load(this, ref _memo);
            set => _memo = value;
        }

        #endregion
        #region Formatting implement

        public override string ToString()
        {
            return $"{nameof(TagId)}: {TagId}, {nameof(MemoId)}: {MemoId}";
        }
        #endregion


        #region LazyLoading

        private CNTag _tag ;

        private CNMemo _memo ;

        private CNMemoTagger(Action<object, string> lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        private Action<object, string> LazyLoader { get; set; }

        #endregion

        #region Constructor

        public CNMemoTagger()
        {
            
        }
        #endregion
    }
}