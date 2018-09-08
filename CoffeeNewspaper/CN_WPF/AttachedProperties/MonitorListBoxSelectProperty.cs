using System.Windows;
using System.Windows.Controls;

namespace CN_WPF
{
    public class MonitorListBoxSelectProperty : BaseAttachedProperty<MonitorListBoxSelectProperty, bool>
    {
        /// <summary>
        ///     When value changed to true bind the proper event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ListBox listBox)) return;
            if ((bool) e.NewValue)
                listBox.SelectionChanged += ListBoxSelectChanged;
            else
                listBox.SelectionChanged -= ListBoxSelectChanged;
        }

        /// <summary>
        ///     When mouse leave the hole parent grid,make it use viewmodel's val to display highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ListBoxSelectChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is ListBox listBox)) return;
            if (listBox.SelectedItem != null)
            {
                listBox.UpdateLayout();
                if (listBox.SelectedItem !=null)
                    listBox.ScrollIntoView(listBox.SelectedItem);
            }
        }
    }
}