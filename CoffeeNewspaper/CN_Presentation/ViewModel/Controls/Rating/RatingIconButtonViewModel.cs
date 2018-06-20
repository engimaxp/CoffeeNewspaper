using System;
using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class RatingIconButtonViewModel:BaseViewModel
    {
        #region Constructor

        public int CurrentPosition { get; set; }
        
        public UserColorType ColorType { get; set; }

        public IconType SolidIcon { get; set; }

        public FontFamilyType SolidFontFamilyType { get; set; }

        public IconType RegularIcon { get; set; }

        public FontFamilyType RegularFontFamilyType { get; set; }

        public bool IsSolidStatus { get; set; }

        public RatingViewModel parentModel { get; set; } 
        
        public IconType CurrentIconType => IsSolidStatus ? SolidIcon : RegularIcon;

        public FontFamilyType CurrentFontFamilyType => IsSolidStatus ? SolidFontFamilyType : RegularFontFamilyType;
        #endregion

        public ICommand RateClickCommand { get; set; }

        public RatingIconButtonViewModel()
        {
            RateClickCommand = new RelayCommand(Rate);
        }

        private void Rate()
        {
            parentModel.SelectedValue = CurrentPosition + 1;
        }
    }
}