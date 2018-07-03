using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CN_Presentation.ViewModel.Base;

namespace CN_WPF
{
    /// <summary>
    ///     ScrollViewExpander.xaml 的交互逻辑
    /// </summary>
    public partial class ScrollViewExpander : UserControl
    {
        #region Constructor

        public ScrollViewExpander()
        {
            InitializeComponent();
        } 
        #endregion

        #region Property Changed Method

        private static object ToggleExpandPropertyChanged(DependencyObject d, object basevalue)
        {
            var expander = d as ScrollViewExpander;
            if (expander?.ContentViewModel == null) return basevalue;
            

            Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if ((bool)basevalue)
                    {
                        expander.ExpanderContent.Content = expander.ContentViewModel.ViewModelToControl();
                        await expander.ExpandScrollView.ScrollViewExpand(false, 0);
                    }

                    else
                    {
                        await expander.ExpandScrollView.ScrollViewShrink(0);
                        expander.ExpanderContent.Content = null;
                    }
                });
            });
            return basevalue;
        }

        #endregion
        #region Denpendency Property

        /// <summary>
        ///     The current expander's ViewModel to show
        /// </summary>
        public BaseViewModel ContentViewModel
        {
            get => (BaseViewModel) GetValue(ContentViewModelProperty);
            set => SetValue(ContentViewModelProperty, value);
        }

        /// <summary>
        ///     Registers <see cref="ContentViewModel" /> as a dependency property
        /// </summary>
        public static readonly DependencyProperty ContentViewModelProperty =
            DependencyProperty.Register(nameof(ContentViewModel), typeof(BaseViewModel), typeof(ScrollViewExpander),
                new UIPropertyMetadata());


        /// <summary>
        ///     The current expander's showing/hiding the content
        /// </summary>
        public bool ToggleExpand
        {
            get => (bool) GetValue(ToggleExpandProperty);
            set => SetValue(ToggleExpandProperty, value);
        }

        /// <summary>
        ///     Registers <see cref="ToggleExpand" /> as a dependency property
        /// </summary>
        public static readonly DependencyProperty ToggleExpandProperty =
            DependencyProperty.Register(nameof(ToggleExpand), typeof(bool), typeof(ScrollViewExpander),
                new UIPropertyMetadata(false, null, ToggleExpandPropertyChanged));

        #endregion
    }
}