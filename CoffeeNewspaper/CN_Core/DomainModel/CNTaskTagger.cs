
namespace CN_Core
{
    /// <summary>
    ///     Relation between tag and task is many-to-one
    /// </summary>
    public class CNTaskTagger
    {
        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string TaskTaggerId { get; set; }

        public string TagId { get; set; }
        public CNTag Tag { get; set; }
        public int TaskId { get; set; }
        public CNTask Task { get; set; }
    }
}