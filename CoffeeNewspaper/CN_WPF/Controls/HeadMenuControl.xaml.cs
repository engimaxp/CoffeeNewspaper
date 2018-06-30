using System.ComponentModel;
using System.Windows.Controls;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_WPF
{
    /// <summary>
    /// HeaderMenuControl.xaml 的交互逻辑
    /// </summary>
    public partial class HeadMenuControl : UserControl
    {
        public HeadMenuControl()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = ViewModelLocator.HeadMenuViewModel;
            }
            else
            {
                DataContext = HeadMenuDesignModel.Instance;
            }
        }
    }
}
