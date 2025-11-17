using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Caliburn.Micro
{
    // tilføjet af mspa

    public class ObservableCollectionExt<T> : ObservableCollection<T>, INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        #region Public Constructors
        public ObservableCollectionExt()
        {
            CollectionChanged += collectionChanged;
        }

        public ObservableCollectionExt(List<T> list) : base((list != null) ? new List<T>(list.Count) : list)
        {
            // Workaround for VSWhidbey bug 562681 (tracked by Windows bug 1369339).
            // We should be able to simply call the base(list) ctor.  But Collection<T>
            // doesn't copy the list (contrary to the documentation) - it uses the
            // list directly as its storage.  So we do the copying here.
            //
            CopyFrom(list);
        }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public ObservableCollectionExt(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CopyFrom(collection);
        }
        #endregion Public Constructors

        #region Public Methods
        /// <summary>
        ///     Refreshes collection by fireing a NotifyCollectionChangedAction.Reset event
        /// </summary>
        public void Refresh()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion

        #region Private Methods
        private void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (IsNotifying)
            Tracer.PropertyChange($"{e.Action}()", GetInvocationList());

            if (e.Action == NotifyCollectionChangedAction.Remove
            || e.Action == NotifyCollectionChangedAction.Replace)
                if (e.OldItems != null)
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= item_PropertyChanged;


            if (e.Action == NotifyCollectionChangedAction.Add
            || e.Action == NotifyCollectionChangedAction.Replace)
                if (e.NewItems != null)
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += item_PropertyChanged;
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = Items;

            if (collection != null && items != null)
            {
                using IEnumerator<T> enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                    items.Add(enumerator.Current);
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (T)sender;
            var index = this.IndexOf(item);

            if (index >= 0)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item, index));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        /// <summary>
        /// Delegate[] list of methods to be call on Event
        /// </summary>        
        /// <returns>List of Methods to be called On Collection Changed</returns>
        public Delegate[] GetInvocationList()
        {
            Type classType = this.GetType();

            FieldInfo eventField = classType.BaseType.GetRuntimeFields().FirstOrDefault(f => f.Name == nameof(CollectionChanged));
            var _delegate = (Delegate)eventField?.GetValue(this);

            return _delegate?.GetInvocationList();
        }
        #endregion Private Methods
    }
}
