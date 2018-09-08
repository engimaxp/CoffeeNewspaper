using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.Input.Design
{
    /// <summary>
    /// The design-time data for a <see cref="TimeRangeEntryViewModel"/>
    /// </summary>
    public class TimeRangeEntryDesignModel : TimeRangeEntryViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TimeRangeEntryDesignModel Instance => new TimeRangeEntryDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TimeRangeEntryDesignModel()
        {
        }

        #endregion
    }
}
