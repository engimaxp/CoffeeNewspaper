
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="UserColorType"/> and returns 
    /// the color brush for that icon
    /// </summary>
    public class ColorTypeToColorConverter : BaseValueConverter<ColorTypeToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom($"#{((UserColorType)value).ToColorText()}"));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
