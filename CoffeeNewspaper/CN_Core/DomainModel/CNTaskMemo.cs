
namespace CN_Core
{
    /// <summary>
    ///     Relation between memo and task is many-to-one
    /// </summary>
    public class CNTaskMemo
    {
        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string TaskMemoId { get; set; }

        public string MemoId { get; set; }
        public CNMemo Memo { get; set; }
        public int TaskId { get; set; }
        public CNTask Task { get; set; }
    }
}