namespace UIShell.OSGi.Collection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UIShell.OSGi.Collection.Locker;

    public sealed class ThreadSafeList<T> : ThreadSafeCollection<T, List<T>, ListLocker<T>>
    {
        public ThreadSafeList() : this(0x2710)
        {
        }

        public ThreadSafeList(int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new List<T>();
        }

        public ThreadSafeList(IEnumerable<T> collection, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new List<T>(collection);
        }

        public ThreadSafeList(int capacity, int millisecondsTimeoutOnLock) : base(millisecondsTimeoutOnLock)
        {
            base.Container = new List<T>(capacity);
        }

        public void Add(T value)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.Add(value);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.AddRange(collection);
            }
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.AsReadOnly();
            }
        }

        public void Clear()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.Clear();
            }
        }

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.ConvertAll<TOutput>(converter);
            }
        }

        public void CopyTo(T[] array)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.CopyTo(array);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.CopyTo(array, arrayIndex);
            }
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.CopyTo(index, array, arrayIndex, count);
            }
        }

        protected override ListLocker<T> CreateLocker() => 
            new ListLocker<T>(this.SyncRoot, base.Container, base.MillisecondsTimeoutOnLock);

        public T Find(Predicate<T> match)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.Find(match);
            }
        }

        public List<T> FindAll(Predicate<T> match)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.FindAll(match);
            }
        }

        public void ForEach(Action<T> action)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.ForEach(action);
            }
        }

        public void Insert(int index, T item)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.Insert(index, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.InsertRange(index, collection);
            }
        }

        public bool Remove(T value)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.Remove(value);
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.RemoveAll(match);
            }
        }

        public T[] ToArray()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.ToArray();
            }
        }

        public void TrimExcess()
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                base.Container.TrimExcess();
            }
        }

        public bool TrueForAll(Predicate<T> match)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                return base.Container.TrueForAll(match);
            }
        }

        public bool TryGet(int index, out T value)
        {
            using (new DisposableLocker(this.SyncRoot, base.MillisecondsTimeoutOnLock))
            {
                if (index < base.Container.Count)
                {
                    value = base.Container[index];
                    return true;
                }
                value = default(T);
                return false;
            }
        }
    }
}

