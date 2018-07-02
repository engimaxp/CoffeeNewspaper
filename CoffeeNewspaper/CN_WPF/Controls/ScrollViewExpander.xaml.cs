using System.ComponentModel;
using System.Windows.Controls;

namespace CN_WPF
{
    /// <summary>
    /// ScrollViewExpander.xaml 的交互逻辑
    /// </summary>
    public partial class ScrollViewExpander : UserControl
    {
        public ScrollViewExpander()
        {
            InitializeComponent();
            ExpanderContent.Content = new TaskExpandDetialControl();
        }
    }
}
