using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CN_WPF
{
    public class TextBoxNumericalAllowOnlyAttachedProperties : BaseAttachedProperty<TextBoxNumericalAllowOnlyAttachedProperties, bool>
    {
        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is TextBox textbox)) return;
            if (!(bool) e.NewValue) return;

            void OnLoaded(object s, RoutedEventArgs ee)
            {
                textbox.Loaded -= OnLoaded;

                //Hook the event
                textbox.PreviewTextInput += (ss, eee) =>
                {
                    eee.Handled = !IsTextAllowed(eee.Text);
                };
            }

            textbox.Loaded += OnLoaded;
        }
    }
}