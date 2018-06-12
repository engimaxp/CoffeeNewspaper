using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// StatisticPage.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticPage : BasePage<StatisticViewModel>
    {
        public StatisticPage()
        {
            InitializeComponent();
        }
        public StatisticPage(StatisticViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
