using System.Windows.Input;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class BubbleSelectViewModel:BaseViewModel
    {

        public BubbleMenuViewModel BubbleMenuViewModel { get; set; }
        
        public FontFamilyType DefaultIconFontFamily { get; set; }

        public IconType DefaultIconFontText { get; set; }

        public UserColorType ForeGroundColor { get; set; } = UserColorType.ForegroundLightDark;
        
        public string ToolTip { get; set; }

        public ICommand ClickCommand { get; set; }

        public BubbleSelectViewModel()
        {
            ClickCommand = new RelayCommand(Click);
        }

        private void Click()
        {
            if (BubbleMenuViewModel != null)
            {
                BubbleMenuViewModel.IsOpen = true;
            }
        }
    }
}