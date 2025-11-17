#if XFORMS
using UIElement = global::Xamarin.Forms.Element;
using FrameworkElement = global::Xamarin.Forms.VisualElement;
using DependencyProperty = global::Xamarin.Forms.BindableProperty;
using DependencyObject = global::Xamarin.Forms.BindableObject;
#elif WINDOWS_UWP
    using Windows.UI.Xaml;
    using Microsoft.Xaml.Interactivity;
#else
using System.Windows;
#endif

namespace Caliburn.Micro
{
    public class NamedDependencyObject
    {
        public DependencyObject Object;
        public string Name;
        //public bool             HasBeenHandled = false;

        public NamedDependencyObject(FrameworkElement src)
        {
            Object = src;
#if !XFORMS
            Name = src.Name;
#endif
        }
        public NamedDependencyObject(DependencyObject src, string name)
        {
            Object = src;
            Name = name;
        }

        public override string ToString() => $"{Name}: {Object}";
    }
}
