
using System;
using System.Globalization;
using System.Windows.Media;
using CN_Presentation;
using CN_Presentation.Utilities;

namespace CN_WPF
{
    /// <summary>
    /// A converter that takes in a <see cref="TaskUrgency"/> and returns 
    /// the ColorBrush for that Indicator
    /// </summary>
    public class TaskUrgencyToBackGroundColorConverter : BaseValueConverter<TaskUrgencyToBackGroundColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserColorType userColor = UserColorType.WordLightBlue;
            if (value != null)
                switch ((TaskUrgency) value)
                {
                    case TaskUrgency.NotUrgent:
                    {
                        userColor = UserColorType.WordGreen;
                        break;
                    }
                    case TaskUrgency.Urgent:
                    {
                        userColor = UserColorType.WordOrange;
                        break;
                    }
                    case TaskUrgency.VeryUrgent:
                    {
                        userColor = UserColorType.WordRed;
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
