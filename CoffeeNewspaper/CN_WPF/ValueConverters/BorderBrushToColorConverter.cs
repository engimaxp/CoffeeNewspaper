
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="Brush"/> and returns 
    /// the Color of that brush
    /// </summary>
    public class BorderBrushToColorConverter : BaseValueConverter<BorderBrushToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
