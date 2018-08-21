using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CN_WPF
{
    /// <summary>
    /// Try to hook the child scroll view to parent scroll view
    /// </summary>
    public class MouseWheelEventBubbleUpAttachedProperty:BaseAttachedProperty<MouseWheelEventBubbleUpAttachedProperty,bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer)) return;
            if ((bool) e.NewValue)
            {
                scrollViewer.LayoutUpdated += (o, args) => {
                    //Hook the event
                    scrollViewer.FindAndActToAllChild<ScrollViewer>((scrollchildview) => { scrollchildview.PreviewMouseWheel += (sss, eee) => PreviewMouseWheel(sss, eee, scrollViewer); });
                };
            }
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e, ScrollViewer scrollViewer)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg =
                    new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent,
                        Source = sender
                    };
                scrollViewer.RaiseEvent(eventArg);
            }
        }
    }
}