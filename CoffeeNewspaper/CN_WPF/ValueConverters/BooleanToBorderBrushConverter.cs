using System;
using System.Globalization;
using System.Windows.Media;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a boolean and returns a borderbrush
    /// </summary>
    public class BooleanToBorderBrushConverter : BaseValueConverter<BooleanToBorderBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool) value)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom($"#{parameter}"));
            }
            else
            {
                return Brushes.Transparent;
            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
