namespace CN_Core
{
    /// <summary>
    ///     Define the current Status of a task
    /// </summary>
    public enum CNTaskStatus
    {
        /// <summary>
        ///     Default Value
        /// </summary>
        UNKONW,

        /// <summary>
        ///     Wait to be start
        /// </summary>
        TODO,

        /// <summary>
        ///     Currently doing this task
        /// </summary>
        DOING,

        /// <summary>
        ///     Task finished
        /// </summary>
        DONE
    }
}