using System.Windows;
using System.Windows.Controls;

namespace CN_WPF
{
    /// <summary>
    /// RatingControl.xaml 的交互逻辑
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public static readonly DependencyProperty DisplayTxtProperty = DependencyProperty.Register(
            nameof(DisplayTxt), typeof(bool), typeof(RatingControl), new PropertyMetadata(false,null,DisplayTxtChanged));

        private static object DisplayTxtChanged(DependencyObject d, object basevalue)
        {
            var rc = d as RatingControl;
            if (rc?.NameColumn == null) return basevalue;
            rc?.NameColumn.SetValue(VisibilityProperty, (bool)basevalue?Visibility.Visible:Visibility.Collapsed);
            return basevalue;
        }

        public bool DisplayTxt
        {
            get { return (bool) GetValue(DisplayTxtProperty); }
            set { SetValue(DisplayTxtProperty, value); }
        }


        public RatingControl()
        {
            InitializeComponent();
        }
    }
}
