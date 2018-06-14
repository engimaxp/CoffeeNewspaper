using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CN_WPF
{
    public class MultiplyConverter : MarkupExtension, IMultiValueConverter
    {

        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static MultiplyConverter Converter;

        public object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
        {
            double result = 1.0;
            foreach (var t in values)
            {
                if (t is double d)
                    result *= d;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Converter ?? (Converter = new MultiplyConverter());
        }
    }
}