namespace UIShell.OSGi.Collection
{
    using System;
    using System.Collections.Generic;
    using UIShell.OSGi.Collection.Locker;

    public class ThreadSafeObservableCollection<T> : ThreadSafeCollection<T, ObservableCollection<T>, ObservableCollectionLocker<T>>
    {
        public event NotifyCollectionChangedEventHandler<T> CollectionChanged;

        public ThreadSafeObservableCollection() : this(0x2710)
        {
        }

        public ThreadSafeObservableCollection(int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new ObservableCollection<T>();
            this.HookEvent();
        }

        public ThreadSafeObservableCollection(IEnumerable<T> collection, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new ObservableCollection<T>(collection);
            this.HookEvent();
        }

        public ThreadSafeObservableCollection(List<T> list, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new ObservableCollection<T>(list);
            this.HookEvent();
        }

        public void Add(T item)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.Add(item);
            }
        }

        public void Clear()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.Clear();
            }
        }

        protected override ObservableCollectionLocker<T> CreateLocker() => 
            new ObservableCollectionLocker<T>(this.SyncRoot, base.Container, base.MillisecondsTimeoutOnLock);

        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                foreach (T local in base.Container)
                {
                    action(local);
                }
            }
        }

        private void HookEvent()
        {
            base.Container.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs<T> e) {
                if (base.CollectionChanged != null)
                {
                    base.CollectionChanged(this, e);
                }
            };
        }

        public bool Remove(T item)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.Remove(item);
            }
        }

        public List<T> ToList()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return new List<T>(base.Container);
            }
        }
    }
}

