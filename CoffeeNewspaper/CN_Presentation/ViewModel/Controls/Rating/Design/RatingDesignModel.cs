using System;
using CN_Core;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class RatingDesignModel:RatingViewModel
    {

        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static RatingDesignModel Instance => new RatingDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RatingDesignModel()
        {
            ColorType = UserColorType.WordOrange;
            RegularIcon = IconType.Star;
            SolidIcon = IconType.Star;
            RegularFontFamilyType = FontFamilyType.FontAwesomeRegular;
            SolidFontFamilyType = FontFamilyType.FontAwesomeSolid;
            SelectedValue = 3;
            EnumType = typeof(CNPriority);
            GenerateChildViewModel();
        }


        #endregion
    }


    public class RatingDesignModel2 : RatingViewModel
    {

        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static RatingDesignModel2 Instance => new RatingDesignModel2();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RatingDesignModel2()
        {
            ColorType = UserColorType.WordRed;
            RegularIcon = IconType.RegularFire;
            SolidIcon = IconType.SolidFire;
            RegularFontFamilyType = FontFamilyType.CNFont;
            SolidFontFamilyType = FontFamilyType.CNFont;
            SelectedValue = 1;
            EnumType = typeof(CNUrgency);
            GenerateChildViewModel();
        }
        #endregion
    }
}