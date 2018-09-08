
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="TaskCurrentStatus"/> and returns 
    /// the FontAwesome for that Indicator
    /// </summary>
    public class TaskStatusToFontCodeConverter : BaseValueConverter<TaskStatusToFontCodeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IconType code = IconType.Times;
            switch ((TaskCurrentStatus)value)
            {
                case TaskCurrentStatus.COMPLETE:
                {
                    code = IconType.Check;
                    break;
                }
                case TaskCurrentStatus.DELETE:
                {
                    code = IconType.Times;
                    break;
                }
                case TaskCurrentStatus.STOP:
                {
                    code = IconType.Pause;
                    break;
                }
                case TaskCurrentStatus.UNDERGOING:
                {
                    code = IconType.Play;
                    break;
                }
                case TaskCurrentStatus.PENDING:
                {
                    code = IconType.HourGlass;
                    break;
                }
            }
            return code.ToFontAwesomeText();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
