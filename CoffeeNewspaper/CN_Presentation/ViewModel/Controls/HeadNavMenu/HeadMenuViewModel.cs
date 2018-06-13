using System.Collections.Generic;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class HeadMenuViewModel : BaseViewModel
    {
        #region Public Properties

        public List<HeadMenuButtonViewModel> NavButtonItems { get; set; }

        #endregion

        public void InformPageChange()
        {
            this.NavButtonItems.ForEach(x => x.InformPageChange());
        }
    }
}