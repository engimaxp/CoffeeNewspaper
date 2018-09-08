using System;
using System.Globalization;
using System.Windows;
using CN_Presentation;

namespace CN_WPF
{
    public class TaskCurrentStatusToVisiblityGoneConverter : BaseValueConverter<TaskCurrentStatusToVisiblityGoneConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return value != null && ((TaskCurrentStatus)value == TaskCurrentStatus.PENDING || (TaskCurrentStatus)value == TaskCurrentStatus.DELETE) ? Visibility.Visible : Visibility.Collapsed;
            else
                return value != null && ((TaskCurrentStatus)value != TaskCurrentStatus.PENDING && (TaskCurrentStatus)value != TaskCurrentStatus.DELETE) ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
