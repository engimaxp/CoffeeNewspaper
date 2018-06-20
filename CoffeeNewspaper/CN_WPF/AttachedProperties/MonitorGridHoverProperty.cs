using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CN_Presentation.ViewModel.Controls;

namespace CN_WPF
{
    public class MonitorGridHoverProperty:BaseAttachedProperty<MonitorGridHoverProperty,bool>
    {
        /// <summary>
        /// When value changed to true bind the proper event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Grid grid)) return;

            grid.MouseEnter -= MouseEnterGrid;
            grid.MouseEnter -= MouseLeaveGrid;
            if ((bool)e.NewValue)
            {
                grid.MouseEnter += MouseEnterGrid;
                grid.MouseLeave += MouseLeaveGrid;
            }
        }

        private void MouseEnterGrid(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            foreach (var gridChild in grid.Children)
            {
                if (gridChild is StackPanel btnPanel)
                {
                    btnPanel.Visibility = Visibility.Visible;
                }
            }
        }
        
        private static void MouseLeaveGrid(object sender, RoutedEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            foreach (var gridChild in grid.Children)
            {
                if (gridChild is StackPanel btnPanel)
                {
                    btnPanel.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}