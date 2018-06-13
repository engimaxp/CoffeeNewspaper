
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="TaskCurrentStatus"/> and returns 
    /// the ColorBrush for that Indicator
    /// </summary>
    public class TaskStatusToForeGroundConverter : BaseValueConverter<TaskStatusToForeGroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserColorType userColor = UserColorType.WordLightBlue;
            if (value != null)
                switch ((TaskCurrentStatus) value)
                {
                    case TaskCurrentStatus.COMPLETE:
                    {
                        userColor = UserColorType.WordGreen;
                        break;
                    }
                    case TaskCurrentStatus.DELETE:
                    {
                        userColor = UserColorType.WordRed;
                        break;
                    }
                    case TaskCurrentStatus.STOP:
                    case TaskCurrentStatus.UNDERGOING:
                    {
                        userColor = UserColorType.WordLightBlue;
                        break;
                    }
                }
            return (SolidColorBrush)(new BrushConverter().ConvertFrom($"#{userColor.ToColorText()}"));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
