namespace UIShell.OSGi.Collection
{
    using System;
    using System.Collections.Generic;
    using UIShell.OSGi.Collection.Locker;

    public class ThreadSafeDictionary<TKey, TValue> : ThreadSafeCollection<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>, DictionaryLocker<TKey, TValue>>
    {
        public ThreadSafeDictionary() : this(0x2710)
        {
        }

        public ThreadSafeDictionary(int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new Dictionary<TKey, TValue>();
        }

        public ThreadSafeDictionary(IDictionary<TKey, TValue> dictionary, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new Dictionary<TKey, TValue>(dictionary);
        }

        public ThreadSafeDictionary(IEqualityComparer<TKey> comparer, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new Dictionary<TKey, TValue>(comparer);
        }

        public ThreadSafeDictionary(int capacity, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new Dictionary<TKey, TValue>(capacity);
        }

        public ThreadSafeDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, int minsecondsTimeoutOnLock) : base(minsecondsTimeoutOnLock)
        {
            base.Container = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public ThreadSafeDictionary(int capacity, IEqualityComparer<TKey> comparer, int minsecondsTimeoutOnLock) : base(minsecondsTimeoutOnLock)
        {
            base.Container = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public void Clear()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.Clear();
            }
        }

        protected override DictionaryLocker<TKey, TValue> CreateLocker() => 
            new DictionaryLocker<TKey, TValue>(this.SyncRoot, base.Container, base.MillisecondsTimeoutOnLock);

        public void ForEach(Action<KeyValuePair<TKey, TValue>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                foreach (KeyValuePair<TKey, TValue> pair in base.Container)
                {
                    action(pair);
                }
            }
        }

        public bool Remove(TKey key)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.Remove(key);
            }
        }

        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
                {
                    return base.Container.Comparer;
                }
            }
        }
    }
}

