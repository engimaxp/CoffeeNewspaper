using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class RatingIconButtonViewModel : BaseViewModel
    {
        #region Constructor

        public RatingIconButtonViewModel()
        {
            RateClickCommand = new RelayCommand(Rate);
        }

        #endregion

        #region Commands

        public ICommand RateClickCommand { get; set; }

        #endregion

        #region Private Methods

        private void Rate()
        {
            ParentModel.SelectedValue = CurrentPosition + 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Current Position
        /// </summary>
        public int CurrentPosition { get; set; }

        /// <summary>
        ///     Color Type
        /// </summary>
        public UserColorType ColorType { get; set; }

        /// <summary>
        ///     Solid Icon font text of this button
        /// </summary>
        public IconType SolidIcon { get; set; }

        /// <summary>
        ///     Solid Icon font family of this button
        /// </summary>
        public FontFamilyType SolidFontFamilyType { get; set; }

        /// <summary>
        ///     Hollow Icon font text of this button
        /// </summary>
        public IconType RegularIcon { get; set; }

        /// <summary>
        ///     Hollow Icon font family of this button
        /// </summary>
        public FontFamilyType RegularFontFamilyType { get; set; }

        /// <summary>
        ///     Indicate currently this button is Solid or Hollow
        /// </summary>
        public bool IsSolidStatus { get; set; }

        /// <summary>
        ///     Parent Whole RatingViewModel to be notified status changed
        /// </summary>
        public RatingViewModel ParentModel { get; set; }

        /// <summary>
        ///     Current Icon Type
        /// </summary>
        public IconType CurrentIconType => IsSolidStatus ? SolidIcon : RegularIcon;

        /// <summary>
        ///     Current Family Type
        /// </summary>
        public FontFamilyType CurrentFontFamilyType => IsSolidStatus ? SolidFontFamilyType : RegularFontFamilyType;

        #endregion
    }
}