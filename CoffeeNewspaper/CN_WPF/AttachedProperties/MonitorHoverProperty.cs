using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CN_Presentation.ViewModel.Controls;

namespace CN_WPF
{
    public class MonitorHoverProperty:BaseAttachedProperty<MonitorHoverProperty,bool>
    {
        /// <summary>
        /// When value changed to true bind the proper event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ItemsControl grid)) return;


            if ((bool)e.NewValue)
            {
                grid.MouseLeave += MouseLeaveGrid;
                // Wait for panel to load
                void OnLoaded(object s, RoutedEventArgs ee)
                {
                    // Unhook
                    grid.Loaded -= OnLoaded;

                    // Loop each child
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        UIElement uiElement = (UIElement) grid.ItemContainerGenerator.ContainerFromIndex(i);
                        uiElement.MouseEnter += MouseEnterButton;
                    }
                }

                // Hook into the Loaded event
                grid.Loaded += OnLoaded;

            }
        }

        /// <summary>
        /// When mouse enter a button,HighLight it and those whose id is lesser,hollow the others
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        private void MouseEnterButton(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!(((ContentPresenter) sender).DataContext is RatingIconButtonViewModel currentHovered)) return;
            currentHovered.ParentModel.SetChildrensSolidStatus(currentHovered.CurrentPosition);
        }

        /// <summary>
        /// When mouse leave the hole parent grid,make it use viewmodel's val to display highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MouseLeaveGrid(object sender, RoutedEventArgs e)
        {
            if (!(sender is ItemsControl grid)) return;
            if (!(grid.DataContext is RatingViewModel ratingModel)) return;
            ratingModel.SetChildrensSolidStatus(ratingModel.SelectedValue-1);
        }
    }
}