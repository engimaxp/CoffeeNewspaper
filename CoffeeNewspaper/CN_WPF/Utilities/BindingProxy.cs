using System;
using System.Windows;
using System.Windows.Markup;

namespace CN_WPF
{
    public class BindingProxy : Freezable
    {
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
    public class BindingProxyValue : MarkupExtension
    {
        public BindingProxy Proxy { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result =  Proxy.Data;
            return result;
        }
    }
}