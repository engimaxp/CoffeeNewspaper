using System.Collections.Generic;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class TagPanelViewModel : BaseViewModel
    {
        public List<TagItemViewModel> TagItems { get; set; }
    }

    public class TagItemViewModel : BaseViewModel
    {
        public string TagTitle { get; set; }
    }
}