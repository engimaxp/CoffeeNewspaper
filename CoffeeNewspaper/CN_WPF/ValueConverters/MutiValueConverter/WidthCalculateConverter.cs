using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CN_WPF
{

    public class WidthCalculateConverter : BaseMutiValueConverter<WidthCalculateConverter>
    {
        public override object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
        {
            double result = 0;
            foreach (var t in values)
            {
                if (t is double d)
                {
                    if (Math.Abs(result) < 1e-5)
                    {
                        result = d;
                    }
                    else
                    {
                        result -= d;
                    }
                }
            }

            return result;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}