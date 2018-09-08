using System.Windows.Controls;
using CN_Presentation.ViewModel.Base;

namespace CN_WPF
{
    /// <summary>
    /// TaskDetailFormControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskDetailFormControl : UserControl
    {
        public TaskDetailFormControl()
        {
            InitializeComponent();
        }

        public TaskDetailFormControl(BaseViewModel VM)
        {
            InitializeComponent();
            this.DataContext = VM;
        }
    }
}
