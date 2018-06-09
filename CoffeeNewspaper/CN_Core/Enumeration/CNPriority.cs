namespace CN_Core
{
    /// <summary>
    ///     Define how urgent a task is
    ///     its a <see cref="short" /> value
    /// </summary>
    public enum CNUrgency : short
    {
        /// <summary>
        ///     Default Value 30
        /// </summary>
        Normal = 30,

        /// <summary>
        ///     Very Urgent 50
        /// </summary>
        VeryHigh = 50,

        /// <summary>
        ///     Urgent 40
        /// </summary>
        High = 40,

        /// <summary>
        ///     Not that urgent
        /// </summary>
        Low = 20,

        /// <summary>
        ///     Anytime do this is ok
        /// </summary>
        VeryLow = 10
    }
}