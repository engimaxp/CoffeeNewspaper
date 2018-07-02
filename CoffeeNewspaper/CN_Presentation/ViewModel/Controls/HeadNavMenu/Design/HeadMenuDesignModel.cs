using System.Collections.Generic;
using CN_Core;

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
            NavButtonItems = IoC.Get<HeadMenuViewModel>().NavButtonItems;
        }

        #endregion
    }


    public class HeadMenuDesignModel2 : HeadMenuViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static HeadMenuDesignModel2 Instance => new HeadMenuDesignModel2();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HeadMenuDesignModel2()
        {
            NavButtonItems = new List<HeadMenuButtonViewModel>()
            {
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.Add,
                },
            };
        }

        #endregion
    }
}
