namespace CN_Core
{
    /// <summary>
    ///     The priority of a task
    ///     value type is <see cref="short" />
    /// </summary>
    public enum CNPriority : short
    {
        /// <summary>
        ///     Default value 30
        /// </summary>
        Normal = 30,

        /// <summary>
        ///     Very important
        /// </summary>
        VeryHigh = 50,

        /// <summary>
        ///     Important
        /// </summary>
        High = 40,

        /// <summary>
        ///     Less important
        /// </summary>
        Low = 20,

        /// <summary>
        ///     Not important at all
        /// </summary>
        VeryLow = 10
    }
}