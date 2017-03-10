namespace UIShell.OSGi.Collection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    [Serializable]
    public class ObservableCollection<T> : Collection<T>
    {
        private SimpleMonitor<T> _monitor;

        public event NotifyCollectionChangedEventHandler<T> CollectionChanged;

        public ObservableCollection()
        {
            this._monitor = new SimpleMonitor<T>();
        }

        public ObservableCollection(IEnumerable<T> collection)
        {
            this._monitor = new SimpleMonitor<T>();
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.CopyFrom(collection);
        }

        public ObservableCollection(List<T> list) : base((list != null) ? new List<T>(list.Count) : list)
        {
            this._monitor = new SimpleMonitor<T>();
            this.CopyFrom(list);
        }

        protected IDisposable BlockReentrancy()
        {
            this._monitor.Enter();
            return this._monitor;
        }

        protected void CheckReentrancy()
        {
            if ((this._monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1))
            {
                throw new InvalidOperationException(SR.GetString("ObservableCollectionReentrancyNotAllowed", new object[0]));
            }
        }

        protected override void ClearItems()
        {
            this.CheckReentrancy();
            base.ClearItems();
            this.OnCollectionReset();
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = base.Items;
            if ((collection != null) && (items != null))
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            this.CheckReentrancy();
            base.InsertItem(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public void Move(int oldIndex, int newIndex)
        {
            this.MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            this.CheckReentrancy();
            T item = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs<T> e)
        {
            if (this.CollectionChanged != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChanged(this, e);
                }
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs<T>(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index, int oldIndex)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs<T>(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T oldItem, T newItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs<T>(action, newItem, oldItem, index));
        }

        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            this.CheckReentrancy();
            T item = base[index];
            base.RemoveItem(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        protected override void SetItem(int index, T item)
        {
            this.CheckReentrancy();
            T oldItem = base[index];
            base.SetItem(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        }

        [Serializable]
        private class SimpleMonitor : IDisposable
        {
            private int _busyCount;

            public void Dispose()
            {
                this._busyCount--;
            }

            public void Enter()
            {
                this._busyCount++;
            }

            public bool Busy =>
                (this._busyCount > 0);
        }
    }
}

