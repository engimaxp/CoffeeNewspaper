namespace CN_Presentation.ViewModel.Controls.Design
{
    public class HeadMenuDesignModel:HeadMenuViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static HeadMenuDesignModel Instance => new HeadMenuDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HeadMenuDesignModel()
        {
            PageSelected = ApplicationPage.WorkSpace;
        }

        #endregion
    }
}
