using System;
using System.Windows;
using System.Windows.Media;

namespace CN_WPF
{
    public static class VisualTreeExtensionClass
    {
        public static void FindAndActToAllChild<T>(this FrameworkElement element,Action<T> doAction) where T : FrameworkElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            var children = new FrameworkElement[childrenCount];

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                children[i] = child;
                if (child is T)
                    doAction((T)child);
            }

            for (int i = 0; i < childrenCount; i++)
                if (children[i] != null)
                {
                    FindAndActToAllChild<T>(children[i], doAction);
                }

        }
        public static T FindFirstChild<T>(this FrameworkElement element) where T : FrameworkElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            var children = new FrameworkElement[childrenCount];

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                children[i] = child;
                if (child is T)
                    return (T)child;
            }

            for (int i = 0; i < childrenCount; i++)
                if (children[i] != null)
                {
                    var subChild = FindFirstChild<T>(children[i]);
                    if (subChild != null)
                        return subChild;
                }

            return null;
        }
    }
}