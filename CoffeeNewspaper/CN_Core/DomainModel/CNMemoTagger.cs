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
        public virtual CNTag Tag { get; set; }
        public string MemoId { get; set; }
        public virtual CNMemo Memo { get; set; }

    #endregion
    #region Formatting implement

    public override string ToString()
        {
            return $"{nameof(TagId)}: {TagId}, {nameof(MemoId)}: {MemoId}";
        }
        #endregion


        
        #region Constructor

        public CNMemoTagger()
        {
            
        }
        #endregion
    }
}