using System.Collections.Generic;

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
            NavButtonItems = new List<HeadMenuButtonViewModel>()
            {
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.BreifCase,
                    TargetPage = ApplicationPage.WorkSpace,
                },
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.Tasks,
                    TargetPage = ApplicationPage.TasksList
                },
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.Notes,
                    TargetPage = ApplicationPage.MemoList
                },
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.ChartArea,
                    TargetPage = ApplicationPage.Statistic
                },
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.GraduationCap,
                    TargetPage = ApplicationPage.TagReview
                },
                new HeadMenuButtonViewModel()
                {
                    FontCode = IconType.Cog,
                    TargetPage = ApplicationPage.Settings
                },
            };
        }

        #endregion
    }
}
