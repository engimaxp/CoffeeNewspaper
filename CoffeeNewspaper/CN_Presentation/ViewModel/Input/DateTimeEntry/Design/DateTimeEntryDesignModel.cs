using System;
using System.Collections.ObjectModel;

namespace CN_Presentation.Input.Design
{
    /// <summary>
    /// The design-time data for a <see cref="DateTimeEntryViewModel"/>
    /// </summary>
    public class DateTimeEntryDesignModel : DateTimeEntryViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static DateTimeEntryDesignModel Instance => new DateTimeEntryDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DateTimeEntryDesignModel()
        {
        }

        #endregion
    }
}
