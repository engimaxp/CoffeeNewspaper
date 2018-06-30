namespace CN_Core
{
    /// <summary>
    /// Store all project scope constants
    /// </summary>
    public class CNConstants
    {
        public const string DIRECTORY_DATEFORMAT = "yyyy-MM-dd";

        /// <summary>
        /// value -1 / means unknow
        /// </summary>
        public const int ESTIMATED_DURATION_NOTKNOWN = -1;
        /// <summary>
        /// value -2 / means user didn't fill the estimated duration property
        /// </summary>
        public const int ESTIMATED_DURATION_NOTWRITE = -2;
        /// <summary>
        /// Pengding reason for task
        /// </summary>
        public const string PENDINGREASON_PreTaskNotComplete = "preTask not finished";
    }
}
