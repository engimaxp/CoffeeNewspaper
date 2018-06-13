
using System;
using System.Globalization;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="IconType"/> and returns 
    /// the FontAwesome string for that icon
    /// </summary>
    public class IconTypeToFontAwesomeConverter : BaseValueConverter<IconTypeToFontAwesomeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                return ((IconType) value).ToFontAwesomeText();
            }
            else
            {
                return ((IconType)value).ToFontAwesomeButton(); 
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
