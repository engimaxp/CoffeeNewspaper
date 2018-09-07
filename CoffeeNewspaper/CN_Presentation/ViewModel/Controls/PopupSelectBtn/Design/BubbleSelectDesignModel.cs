using System.Collections.ObjectModel;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class BubbleSelectDesignModel : BubbleSelectViewModel
    {
        #region Constructor

        /// <summary>
        ///     Default constructor
        /// </summary>
        public BubbleSelectDesignModel()
        {
            DefaultIconFontFamily = FontFamilyType.FontAwesomeSolid;
            DefaultIconFontText = IconType.Filter;
            ToolTip = "Filter";
            BubbleMenuViewModel = new BubbleMenuViewModel
            {
                Buttons = new ObservableCollection<BubbleMenuButtonViewModel>
                {
                    new BubbleMenuButtonViewModel
                    {
                        ButtonText = "Display All",
                        IconFontFamily = FontFamilyType.FontAwesomeSolid,
                        IconFontText = IconType.ThLarge
                    },
                    new BubbleMenuButtonViewModel
                    {
                        ButtonText = "Dont Display Fail",
                        IconFontFamily = FontFamilyType.FontAwesomeSolid,
                        IconFontText = IconType.Times
                    },
                    new BubbleMenuButtonViewModel
                    {
                        ButtonText = "Dont Display Complete",
                        IconFontFamily = FontFamilyType.FontAwesomeSolid,
                        IconFontText = IconType.Check
                    },
                    new BubbleMenuButtonViewModel
                    {
                        ButtonText = "Dont Display Pending",
                        IconFontFamily = FontFamilyType.FontAwesomeSolid,
                        IconFontText = IconType.HourGlass
                    }
                }
            };
        }

        #endregion

        #region Singleton

        /// <summary>
        ///     A single instance of the design model
        /// </summary>
        public static BubbleSelectDesignModel Instance => new BubbleSelectDesignModel();

        #endregion
    }
}