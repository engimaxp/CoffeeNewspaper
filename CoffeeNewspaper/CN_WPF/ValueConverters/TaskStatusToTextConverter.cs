
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="TaskCurrentStatus"/> and returns 
    /// the text for that Indicator
    /// </summary>
    public class TaskStatusToTextConverter : BaseValueConverter<TaskStatusToTextConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = string.Empty;
            switch ((TaskCurrentStatus)value)
            {
                case TaskCurrentStatus.COMPLETE:
                {
                    code = "Complete";
                    break;
                }
                case TaskCurrentStatus.DELETE:
                {
                    code = "Delete";
                        break;
                }
                case TaskCurrentStatus.STOP:
                {
                    code = "Stop";
                        break;
                }
                case TaskCurrentStatus.UNDERGOING:
                {
                    code = "Undergoing";
                    break;
                }
                case TaskCurrentStatus.PENDING:
                {
                    code = "Pending";
                    break;
                }
            }
            return code;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
