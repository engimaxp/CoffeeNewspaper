
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="FontFamilyType"/> and returns 
    /// the fontfamily instance for that text element
    /// </summary>
    public class FontFamilyTypeToFontFamilyConverter : BaseValueConverter<FontFamilyTypeToFontFamilyConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"pack://application;,,,/Fonts/#{((FontFamilyType)value).ToFontFamilyNameText()}";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
