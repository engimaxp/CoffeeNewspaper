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
        public CNTag Tag { get; set; }
        public string MemoId { get; set; }
        public CNMemo Memo { get; set; }

        #endregion
        #region Formatting implement

        public override string ToString()
        {
            return $"{nameof(TagId)}: {TagId}, {nameof(MemoId)}: {MemoId}";
        } 
        #endregion
    }
}