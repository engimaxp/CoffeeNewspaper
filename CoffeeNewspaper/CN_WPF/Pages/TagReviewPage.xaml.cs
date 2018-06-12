using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// TagReviewPage.xaml 的交互逻辑
    /// </summary>
    public partial class TagReviewPage : BasePage<TagReviewViewModel>
    {
        public TagReviewPage()
        {
            InitializeComponent();
        }
        public TagReviewPage(TagReviewViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
