using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : BasePage<SettingsViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        public SettingsPage(SettingsViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
