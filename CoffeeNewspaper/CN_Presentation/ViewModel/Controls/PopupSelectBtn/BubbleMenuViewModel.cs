using System.Collections.ObjectModel;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class BubbleMenuViewModel:BaseViewModel
    {
        public bool IsOpen { get; set; }

        public ObservableCollection<BubbleMenuButtonViewModel> Buttons { get; set; } = new ObservableCollection<BubbleMenuButtonViewModel>();
    }
}