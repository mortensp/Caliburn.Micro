using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Caliburn.Micro
{
    // ----------------------------------------------------------------------------------------------
    //mspa: Added IsNotitfying property and these methods:                                          -
    //      TurnOfNortification()   => returns the last value of IsNotitfying before turning it off -
    //      GetInvocationList()     => List subscriber methods                                      -
    // ----------------------------------------------------------------------------------------------


    /// <summary>
    /// A base collection class that supports automatic UI thread marshalling.
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the collection.</typeparam>
    public class BindableCollection<T> : ObservableCollection<T>, IObservableCollection<T>
    {
        private void BindableCollection_PropertyChanged(object sender, PropertyChangedEventArgs e) => throw new NotImplementedException();
        /// <summary>
        /// Gets a value indicating whether all property change notifications are turned off globally.
        /// </summary>
        public bool IsAllNotificationTurnedOff => PropertyChangedBase.IsAllNotificationTurnedOff;

        /// <summary>
        /// Initializes a new instance of the <see cref = "BindableCollection&lt;T&gt;" /> class.
        /// </summary>
        public BindableCollection()
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "BindableCollection&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "collection">The collection from which the elements are copied.</param>
        public BindableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Enables/Disables property change notification.
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsNotifying { get; set; }
        /// <summary>
        /// Disables notifications and returns the previous notification state.
        /// </summary>
        /// <remarks>If notifications are already disabled, calling this method has no effect other than
        /// returning the current state. This method can be used to atomically check and change the notification
        /// status.</remarks>
        /// <returns>true if notifications were enabled before the call; otherwise, false.</returns>
        public bool TurnOffNotification()
        {
            var status = IsNotifying;
            IsNotifying = false;
            return status;
        }
        /// <summary>
        /// Enables notifications and returns the previous notification state.
        /// </summary>
        /// <remarks>If notifications are already enabled, this method has no effect on the notification
        /// state. Use this method to ensure notifications are active and to determine whether they were previously
        /// enabled.</remarks>
        /// <returns>true if notifications were already enabled; otherwise, false.</returns>

        public bool TurnOnNotification()
        {
            var status = IsNotifying;
            IsNotifying = true;
            return status;
        }
        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            if (IsNotifying && !IsAllNotificationTurnedOff)
            {
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
        /// Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public void Refresh()
        {
            IsNotifying = true;

            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(() =>
                {
                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                });
            }
            else
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Inserts the item to the specified position.
        /// </summary>
        /// <param name = "index">The index to insert at.</param>
        /// <param name = "item">The item to be inserted.</param>
        protected override sealed void InsertItem(int index, T item)
        {
            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(() => InsertItemBase(index, item));
            }
            else
            {
                InsertItemBase(index, item);
            }
        }

        /// <summary>
        /// Exposes the base implementation of the <see cref = "InsertItem" /> function.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <param name = "item">The item.</param>
        /// <remarks>
        /// Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void InsertItemBase(int index, T item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Sets the item at the specified position.
        /// </summary>
        /// <param name = "index">The index to set the item at.</param>
        /// <param name = "item">The item to set.</param>
        protected override sealed void SetItem(int index, T item)
        {
            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(() => SetItemBase(index, item));
            }
            else
            {
                SetItemBase(index, item);
            }
        }

        /// <summary>
        /// Exposes the base implementation of the <see cref = "SetItem" /> function.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <param name = "item">The item.</param>
        /// <remarks>
        /// Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void SetItemBase(int index, T item)
        {
            base.SetItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified position.
        /// </summary>
        /// <param name = "index">The position used to identify the item to remove.</param>
        protected override sealed void RemoveItem(int index)
        {
            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(() => RemoveItemBase(index));
            }
            else
            {
                RemoveItemBase(index);
            }
        }

        /// <summary>
        /// Exposes the base implementation of the <see cref = "RemoveItem" /> function.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <remarks>
        ///   Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void RemoveItemBase(int index)
        {
            base.RemoveItem(index);
        }

        /// <summary>
        /// Clears the items contained by the collection.
        /// </summary>
        protected override sealed void ClearItems()
        {
            OnUIThread(ClearItemsBase);
        }

        /// <summary>
        /// Exposes the base implementation of the <see cref = "ClearItems" /> function.
        /// </summary>
        /// <remarks>
        ///   Used to avoid compiler warning regarding unverifiable code.
        /// </remarks>
        protected virtual void ClearItemsBase()
        {
            base.ClearItems();
        }

        /// <summary>
        /// Raises the <see cref = "E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name = "e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsNotifying && !IsAllNotificationTurnedOff)
            {
                Tracer.PropertyChange($"{e.Action}()", GetInvocationList());
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event with the provided arguments.
        /// </summary>
        /// <param name = "e">The event data to report in the event.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (IsNotifying && !IsAllNotificationTurnedOff)
            {
                Tracer.PropertyChange($"{e.PropertyName}", GetInvocationList());
                base.OnPropertyChanged(e);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            void AddRange()
            {
                var previousNotificationSetting = IsNotifying;
                IsNotifying = false;
                var index = Count;
                foreach (var item in items)
                {
                    InsertItemBase(index, item);
                    index++;
                }
                IsNotifying = previousNotificationSetting;

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(AddRange);
            }
            else
            {
                AddRange();
            }
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void RemoveRange(IEnumerable<T> items)
        {
            void RemoveRange()
            {
                var previousNotificationSetting = IsNotifying;
                IsNotifying = false;
                foreach (var item in items)
                {
                    var index = IndexOf(item);
                    if (index >= 0)
                    {
                        RemoveItemBase(index);
                    }
                }
                IsNotifying = previousNotificationSetting;

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(RemoveRange);
            }
            else
            {
                RemoveRange();
            }
        }

        /// <summary>
        /// Executes the given action on the UI thread
        /// </summary>
        /// <remarks>An extension point for subclasses to customise how property change notifications are handled.</remarks>
        /// <param name="action"></param>
        protected virtual void OnUIThread(System.Action action) => action.OnUIThread();

        /// <summary>
        /// Delegate[] list of methods to be call on Event
        /// Note: Is only intended for the Tracer and debugging
        /// </summary>        
        /// <returns>List of Methods to be called On Collection Changed</returns>
        //mspa: 2024-06-03 Added GetInvocationList() method
        public Delegate[] GetInvocationList()
        {
            Type classType = this.GetType();

            FieldInfo eventField = classType.BaseType.GetRuntimeFields().FirstOrDefault(f => f.Name == nameof(CollectionChanged));

            return ((Delegate)eventField?.GetValue(this))?.GetInvocationList();
        }
   }
}
