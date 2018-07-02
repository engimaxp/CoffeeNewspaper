using System;
using System.Collections.Generic;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class RatingViewModel : BaseViewModel
    {
        #region Public Methods

        public RatingViewModel GenerateChildViewModel(int MaximumIconNum)
        {
            if(MaximumIconNum<this.SelectedValue) throw new ArgumentException("Invalid parameter MaximumIconNum");
            IconButtons = new List<RatingIconButtonViewModel>();
            for (var i = 0; i < MaximumIconNum; i++)
                IconButtons.Add(new RatingIconButtonViewModel
                {
                    ColorType = ColorType,
                    CurrentPosition = i,
                    SolidIcon = SolidIcon,
                    SolidFontFamilyType = SolidFontFamilyType,
                    RegularIcon = RegularIcon,
                    RegularFontFamilyType = RegularFontFamilyType,
                    IsSolidStatus = SelectedValue - 1 >= i,
                    parentModel = this
                });
            return this;
        }

        #endregion

        #region Public Properties

        public List<RatingIconButtonViewModel> IconButtons { get; set; }

        public IconType SolidIcon { get; set; }

        public FontFamilyType SolidFontFamilyType { get; set; }

        public IconType RegularIcon { get; set; }

        public FontFamilyType RegularFontFamilyType { get; set; }

        public UserColorType ColorType { get; set; }

        public int SelectedValue { get; set; }

        #endregion

        public void SetChildrensSolidStatus(int currentHoveredCurrentPosition)
        {
            foreach (var ratingIconButtonViewModel in IconButtons)
            {
                ratingIconButtonViewModel.IsSolidStatus =
                    currentHoveredCurrentPosition >= ratingIconButtonViewModel.CurrentPosition;
            }
        }
    }
}