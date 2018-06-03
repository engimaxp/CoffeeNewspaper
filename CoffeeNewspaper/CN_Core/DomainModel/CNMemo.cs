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
        public virtual ICollection<CNTaskMemo> TaskMemos { get; set; } = new HashSet<CNTaskMemo>();

        /// <summary>
        ///     Tags of this Memo
        /// </summary>
        public virtual ICollection<CNMemoTagger> MemoTaggers { get; set; } = new HashSet<CNMemoTagger>();

        #endregion
    }
}