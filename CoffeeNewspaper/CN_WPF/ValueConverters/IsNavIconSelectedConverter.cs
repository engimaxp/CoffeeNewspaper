using System;
using System.Globalization;
using CN_Presentation;

namespace CN_WPF
{
    /// <summary>
    ///     A converter that takes in a enumtype and returns the nav icon is selected
    /// </summary>
    public class IsNavIconSelectedConverter : BaseValueConverter<IsNavIconSelectedConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var currentPage = (ApplicationPage) Enum.Parse(typeof(ApplicationPage), value.ToString());
                if (parameter != null)
                {
                    var iconPage = (ApplicationPage)Enum.Parse(typeof(ApplicationPage), parameter.ToString());
                    return currentPage == iconPage;
                }
            }

            return false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}