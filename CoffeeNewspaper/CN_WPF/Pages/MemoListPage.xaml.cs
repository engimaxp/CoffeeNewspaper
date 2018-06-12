using System.Windows.Controls;
using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// MemoListPage.xaml 的交互逻辑
    /// </summary>
    public partial class MemoListPage : BasePage<MemoListViewModel>
    {
        public MemoListPage()
        {
            InitializeComponent();
        }
        public MemoListPage(MemoListViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
