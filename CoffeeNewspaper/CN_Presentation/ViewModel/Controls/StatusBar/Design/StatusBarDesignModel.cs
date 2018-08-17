using CN_Presentation.ViewModel.Controls.StatusBar;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class StatusBarDesignModel : StatusBarViewModel
    {

        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static StatusBarDesignModel Instance => new StatusBarDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusBarDesignModel()
        {
        }


        #endregion
    }
    
}