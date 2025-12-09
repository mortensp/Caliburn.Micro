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
    /// <summary>
    /// Represents a dependency object paired with an associated name, typically used to identify or reference UI
    /// elements within a collection or data structure.
    /// </summary>
    /// <remarks>This class is useful for scenarios where dependency objects need to be tracked or referenced
    /// by name, such as in UI frameworks that support named elements. The <see cref="Object"/> field holds the
    /// dependency object, while the <see cref="Name"/> field stores its associated name. Instances can be constructed
    /// from either a <see cref="FrameworkElement"/> (using its <c>Name</c> property) or any <see
    /// cref="DependencyObject"/> with an explicit name.</remarks>
    public class NamedDependencyObject
    {
        /// <summary>
        /// The dependency object associated with this Named instance.
        /// </summary>  
        public DependencyObject Object;
        /// <summary>
        /// Gets or sets the name associated with this Named instance.
        /// </summary>
        public string Name;
        //public bool             HasBeenHandled = false;
        /// <summary>
        /// Initializes a new instance of the NamedDependencyObject class using the specified FrameworkElement as the
        /// underlying object.
        /// </summary>
        /// <remarks>If the provided FrameworkElement has a non-empty Name property, it will be used to
        /// set the Name of the NamedDependencyObject, except on platforms where this property is not
        /// available.</remarks>
        /// <param name="src">The FrameworkElement to associate with this NamedDependencyObject. Cannot be null.</param>
        public NamedDependencyObject(FrameworkElement src)
        {
            Object = src;
#if !XFORMS
            Name = src.Name;
#endif
        }
        /// <summary>
        /// Initializes a new instance of the NamedDependencyObject class with the specified dependency object and name.
        /// </summary>
        /// <param name="src">The source DependencyObject to associate with this instance. Cannot be null.</param>
        /// <param name="name">The name to assign to the dependency object. Cannot be null or empty.</param>
        public NamedDependencyObject(DependencyObject src, string name)
        {
            Object = src;
            Name = name;
        }
        /// <summary>
        ///Returns a string representation of the NamedDependencyObject, including its name and associated object.   
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name}: {Object}";
    }
}
