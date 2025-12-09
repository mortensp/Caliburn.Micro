using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Caliburn.Micro
{
    // ----------------------------------------------------------------------------------------------
    //mspa: Added IsNotitfying property and these methods:                                          -
    //      TurnOfNortification()   => returns the last value of IsNotitfying before turning it off -
    //      GetInvocationList()     => List subscriber methods                                      -
    //      NotMapped and JsonIgnore attributes has been added
    // ----------------------------------------------------------------------------------------------
    
    /// <summary>
    /// A base class that implements the infrastructure for property change notification and automatically performs UI thread marshalling.
    /// </summary>
    [DataContract]
    public class PropertyChangedBase : INotifyPropertyChangedEx
    {
        /// <summary>
        /// Creates an instance of <see cref = "PropertyChangedBase" />.
        /// </summary>
        public PropertyChangedBase()
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Enables/Disables property change notification.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public virtual bool IsNotifying { get; set; }

        /// <summary>
        /// Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public virtual void Refresh()
        {
            NotifyOfPropertyChange(string.Empty);
        }

        #region NotifyOfPropertyChange Methods
        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (IsNotifying && !IsAllNotificationTurnedOff)
            {
                if (PropertyChanged is null)                
                    Tracer.PropertyChange(propertyName, PropertyChanged?.GetInvocationList());                
                else
                    if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
                    {
                        OnUIThread(() => OnPropertyChanged(new PropertyChangedEventArgs(propertyName)));
                        }
                    else
                    {
                        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                        }
            }
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name = "TProperty">The type of the property.</typeparam>
        /// <param name = "property">The property expression.</param>
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }
         
              /// <summary>
            /// Notifies subscribers of the property change.
            /// </summary>
            /// <param name = "classPropertyName">Name of the class-property.</param>
            /// <param name = "propertyName">Name of the property.</param>
            //mspa: 
            public virtual void NotifyOfPropertyChange(string classPropertyName, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            {
                if (IsNotifying && !IsAllNotificationTurnedOff)
                    if (PropertyChanged is null)
                        Tracer.PropertyChange($"{classPropertyName} ({propertyName})", PropertyChanged?.GetInvocationList());
                    else
                        if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
                            OnUIThread(() => OnPropertyChanged(new PropertyChangedEventArgs(classPropertyName)));
                        else
                            OnPropertyChanged(new PropertyChangedEventArgs(classPropertyName));
            }

        #endregion

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event directly.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (IsNotifying && !IsAllNotificationTurnedOff)
                Tracer.PropertyChange(e.PropertyName, PropertyChanged?.GetInvocationList());

            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Executes the given action on the UI thread
        /// </summary>
        /// <remarks>An extension point for subclasses to customise how property change notifications are handled.</remarks>
        /// <param name="action"></param>
        protected virtual void OnUIThread(System.Action action) => action.OnUIThread();

        /// <summary>
        /// Sets a backing field value and if it's changed raise a notification.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="oldValue">A reference to the field to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property for change notifications.</param>
        /// <returns></returns>
        public virtual bool Set<T>(ref T oldValue, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            oldValue = newValue;

            NotifyOfPropertyChange(propertyName ?? string.Empty);

            return true;
        }
        /// <summary>
        /// Setup Bubling of PropertyChanged events to parent
        /// </summary>
        /// <param name = "backingField">Name of the propertys BackingField.</param>
        /// <param name = "value">New property value.</param>
        /// <param name = "classPropertyName">Name of the Class-property.</param>
        /// <returns></returns>
        //mspa:
        public void BubblePropertyChanged(PropertyChangedBase backingField, PropertyChangedBase value, [System.Runtime.CompilerServices.CallerMemberName] string classPropertyName = null)
        {
            if (backingField != null)
                backingField.PropertyChanged -= noitifyParent;

            if (value != null)
                value.PropertyChanged += noitifyParent;

            void noitifyParent(object sender, PropertyChangedEventArgs e)
            {
                NotifyOfPropertyChange(classPropertyName, e.PropertyName);
            }
        }

      /// <summary>
      /// Retrieves the list of delegates currently subscribed to the PropertyChanged event.
      /// </summary>
      /// <remarks>The returned array reflects the current state of the event's subscription. Modifying the
      /// array does not affect the event's invocation list.</remarks>
      /// <returns>An array of Delegate objects representing the invocation list of the PropertyChanged event. Returns an empty
      /// array if no handlers are attached.</returns>
        public Delegate[] GetInvocationList() => PropertyChanged?.GetInvocationList();

        /// <summary>
        /// Gets or sets a value indicating whether all notifications are turned off for the application.
        /// </summary>
        /// <remarks>This property is static and applies globally across all instances. Changes to this
        /// value affect notification behavior for all users and sessions.</remarks>
        [NotMapped]
        [JsonIgnore]
        public static  bool IsAllNotificationTurnedOff { get; set; } = false;
        /// <summary>
        /// Turns off all notifications for the application and returns the previous notification state.
        /// </summary>
        /// <remarks>This method sets the global notification state to off. Subsequent calls will continue
        /// to return the previous state before the change. This method is not thread-safe; concurrent calls may result
        /// in unexpected behavior.</remarks>
        /// <returns>true if all notifications were already turned off before this call; otherwise, false.</returns>
        public static bool TurnOffAllNortification()
        {
            var status                 = IsAllNotificationTurnedOff;
            IsAllNotificationTurnedOff = true;
            return status;
        }
        /// <summary>
        /// Restores the global notification setting to its previous state.
        /// </summary>
        /// <param name="oldStatus">The previous notification status to restore. Specify <see langword="true"/> to enable all notifications;
        /// otherwise, <see langword="false"/> to disable them.</param>
        public static void RestoreAllNotification(bool oldStatus)
        {
            IsAllNotificationTurnedOff = oldStatus;
        }
        /// <summary>
        /// Disables notifications and returns the previous notification state.
        /// </summary>
        /// <returns>true if notifications were enabled before the method was called; otherwise, false.</returns>
        public bool TurnOffNortification()
        {
            var status  = IsNotifying;
            IsNotifying = false;
            return status;
        }
        /// <summary>
        /// Restores the notification status to a previous value.
        /// </summary>
        /// <param name="oldStatus">The notification status to restore. Specify <see langword="true"/> to enable notifications; otherwise, <see
        /// langword="false"/>.</param>
        public void RestoreNotification(bool oldStatus)
        {
            IsNotifying = oldStatus;
        }
    }
}
