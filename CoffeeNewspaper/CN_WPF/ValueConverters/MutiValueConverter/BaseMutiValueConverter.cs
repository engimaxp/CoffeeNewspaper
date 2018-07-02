using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CN_WPF
{
    public abstract class BaseMutiValueConverter<T> : MarkupExtension, IMultiValueConverter where T:class,new()
    {
        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static readonly T Converter = new T();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Converter ;
        }

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}