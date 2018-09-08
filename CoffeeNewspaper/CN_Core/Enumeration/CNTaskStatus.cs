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
        ///     this task has some preTasks Undone or this task need other resource currently not available
        /// </summary>
        PENDING,
        /// <summary>
        ///     Task finished
        /// </summary>
        DONE
    }
}