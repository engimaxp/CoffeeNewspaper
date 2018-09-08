using System.Windows.Controls;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;

namespace CN_WPF
{
    /// <summary>
    /// TaskExpandDetialControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskExpandDetialControl : UserControl
    {
        public TaskExpandDetialControl()
        {
            InitializeComponent();
        }

        public TaskExpandDetialControl(TaskExpandDetailViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
