using CN_Presentation.ViewModel.Input;

namespace CN_Presentation.Input.Design
{
    /// <summary>
    /// The design-time data for a <see cref="TimeSpanPickerViewModel"/>
    /// </summary>
    public class TimeSpanPickerDesignModel : TimeSpanPickerViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TimeSpanPickerDesignModel Instance => new TimeSpanPickerDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TimeSpanPickerDesignModel()
        {
        }

        #endregion
    }
}
