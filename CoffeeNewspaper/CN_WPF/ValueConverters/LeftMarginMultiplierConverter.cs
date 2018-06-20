using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CN_WPF
{
    public class LeftMarginMultiplierConverter:BaseValueConverter<LeftMarginMultiplierConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TreeViewItem item))
                return new Thickness(0);
            //Default value
            double Length = 19;

            if (parameter != null)
                Double.TryParse(parameter.ToString(), out Length);
            var thickness = Length * item.GetDepth();
            return new Thickness(thickness, 0, 0, 0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WidthMultiplierConverter : BaseValueConverter<WidthMultiplierConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TreeViewItem item))
                return 300;
            //Default value
            double Length = 19;

            if (parameter != null)
                Double.TryParse(parameter.ToString(), out Length);

            return Length * item.GetDepth();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}