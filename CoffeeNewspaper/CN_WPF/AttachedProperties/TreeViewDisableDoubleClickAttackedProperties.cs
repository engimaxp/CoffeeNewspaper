using System.Windows;
using System.Windows.Controls;

namespace CN_WPF
{
    public class TreeViewDisableDoubleClickAttackedProperties:BaseAttachedProperty<TreeViewDisableDoubleClickAttackedProperties,bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is TreeView treeView)) return;
            if (!(bool) e.NewValue) return;

            void OnLoaded(object s, RoutedEventArgs ee)
            {
                treeView.Loaded -= OnLoaded;

                //Hook the event
                treeView.PreviewMouseLeftButtonDown += (ss, eee) =>
                {
                    if (eee.ClickCount > 1)
                    {
                        //here you would probably want to include code that is called by your
                        //mouse down event handler.
                        eee.Handled = true;
                    }
                };
            }

            treeView.Loaded += OnLoaded;
        }
    }
}