namespace UIShell.OSGi.Collection.Locker
{
    using System;
    using Collection;

    public class ObservableCollectionLocker<T> : EnumerableLocker<T, ObservableCollection<T>>
    {
        public ObservableCollectionLocker(object syncRoot, ObservableCollection<T> collection, int millisecondsTimeout)
            : base(syncRoot, collection, millisecondsTimeout)
        {
        }

        public bool Contains(T item) => Container.Contains(item);

        public void CopyTo(T[] array, int index)
        {
            Container.CopyTo(array, index);
        }

        public int IndexOf(T item) => Container.IndexOf(item);

        public void Insert(int index, T item)
        {
            Container.Insert(index, item);
        }

        public void RemoveAll(Predicate<T> match)
        {
            for (int i = 0; i < Container.Count; i++)
            {
                if (match(Container[i]))
                {
                    RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveAt(int index)
        {
            Container.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return Container[index];
            }
            set
            {
                Container[index] = value;
            }
        }
    }
}

