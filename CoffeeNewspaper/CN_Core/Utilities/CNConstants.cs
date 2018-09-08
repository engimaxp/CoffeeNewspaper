namespace CN_Core
{
    /// <summary>
    /// Store all project scope constants
    /// </summary>
    public static class CNConstants
    {
        /// <summary>
        /// Ninject scope for service
        /// one service get will create one dbcontext ,for more info see https://github.com/ninject/Ninject.Extensions.NamedScope/wiki/InNamedScope
        /// </summary>
        public const string NinjectNamedScopeForService = "Service";

        /// <summary>
        /// Date format yyyy-MM-dd
        /// </summary>
        public const string DIRECTORY_DATEFORMAT = "yyyy-MM-dd";

        /// <summary>
        /// Date time format yyyy-MM-dd HH-mm-ss
        /// </summary>
        public const string DIRECTORY_DATETIMEFORMAT = "yyyy-MM-dd HH:mm:ss";

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

        /// <summary>
        /// Mutiplier for convert from one second to ticks
        /// </summary>
        public const int OneSecondToTickUnit = 10000000;
    }
}
