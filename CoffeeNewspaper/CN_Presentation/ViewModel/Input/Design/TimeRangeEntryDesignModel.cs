using System;
using System.Collections.ObjectModel;

namespace CN_Presentation.Input.Design
{
    /// <summary>
    /// The design-time data for a <see cref="DateTimeEntryViewModel"/>
    /// </summary>
    public class TimeRangeEntryDesignModel : DateTimeEntryViewModel
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
