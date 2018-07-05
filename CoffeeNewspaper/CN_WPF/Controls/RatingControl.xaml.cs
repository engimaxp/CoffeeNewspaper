using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
