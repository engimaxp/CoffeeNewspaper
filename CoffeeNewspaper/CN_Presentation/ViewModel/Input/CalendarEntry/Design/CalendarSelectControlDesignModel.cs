using System.Collections.Generic;
using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.Input.Design
{
    /// <summary>
    /// The design-time data for a <see cref="CalendarSelectControlViewModel"/>
    /// </summary>
    public class CalendarSelectControlDesignModel : CalendarSelectControlViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static CalendarSelectControlDesignModel Instance => new CalendarSelectControlDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CalendarSelectControlDesignModel()
        {
        }

        #endregion
    }
}
