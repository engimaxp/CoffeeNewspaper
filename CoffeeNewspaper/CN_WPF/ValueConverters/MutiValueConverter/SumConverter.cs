using System;
using System.Globalization;

namespace CN_WPF
{
    public class SumConverter : BaseMutiValueConverter<SumConverter>
    {
        
        public override object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
        {
            double result = 1.0;
            foreach (var t in values)
            {
                if (t is double d)
                    result += d;
            }

            return result;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}