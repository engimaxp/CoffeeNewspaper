using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// WorkSpacePage.xaml 的交互逻辑
    /// </summary>
    public partial class WorkSpacePage : BasePage<WorkSpaceViewModel>
    {
        public WorkSpacePage()
        {
            InitializeComponent();
        }

        public WorkSpacePage(WorkSpaceViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
