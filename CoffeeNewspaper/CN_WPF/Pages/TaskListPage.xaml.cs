using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// TaskListPage.xaml 的交互逻辑
    /// </summary>
    public partial class TaskListPage : BasePage<TasksListViewModel>
    {
        public TaskListPage()
        {
            InitializeComponent();
        }
        public TaskListPage(TasksListViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
