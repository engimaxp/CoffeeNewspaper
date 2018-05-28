
namespace CN_Core
{
    /// <summary>
    ///     Relation between pretask and suftask is many-to-many
    /// </summary>
    public class CNTaskConnector
    {
        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string TaskConnectorId { get; set; }

        public int PreTaskId { get; set; }
        public CNTask PreTask { get; set; }

        public int SufTaskId { get; set; }
        public CNTask SufTask { get; set; }
    }
}