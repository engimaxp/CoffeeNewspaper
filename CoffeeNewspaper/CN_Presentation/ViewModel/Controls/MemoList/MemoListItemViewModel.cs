using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_Presentation.ViewModel.Controls
{
    public class MemoListItemViewModel:BaseViewModel
    {

        public TagPanelViewModel TagPanelViewModel { get; set; } = TagPanelDesignModel.Instance;
    }
}