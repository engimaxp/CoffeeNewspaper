using System;
using System.Diagnostics;
using System.Globalization;
using CN_Presentation.ViewModel.Form;

namespace CN_WPF
{
    public class FormControlValueConverter : BaseValueConverter<FormControlValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskDetailFormViewModel formViewModel)
            {
                return new TaskDetailFormControl(formViewModel);
            }
            else
            {
                Debugger.Break();
                return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}