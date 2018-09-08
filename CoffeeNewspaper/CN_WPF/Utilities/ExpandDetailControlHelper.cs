using System.Diagnostics;
using System.Windows.Controls;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;

namespace CN_WPF
{
    public static class ExpandDetailControlHelper
    {
        public static UserControl ViewModelToControl(this BaseViewModel viewModel)
        {
            if (viewModel is TaskExpandDetailViewModel model)
            {
                return new TaskExpandDetialControl(model);
            }
            else
            {
                Debugger.Break();
                return null;
            }
        }
    }
}
