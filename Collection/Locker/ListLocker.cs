namespace UIShell.OSGi.Collection.Locker
{
    using System;
    using System.Collections.Generic;

    public class ListLocker<T> : EnumerableLocker<T, List<T>>
    {
        public ListLocker(object syncRoot, List<T> list, int millisecondsTimeout)
            : base(syncRoot, list, millisecondsTimeout)
        {
        }

        public int BinarySearch(T item) =>
            Container.BinarySearch(item);

        public int BinarySearch(T item, IComparer<T> comparer) =>
            Container.BinarySearch(item, comparer);

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) =>
            Container.BinarySearch(index, count, item, comparer);

        public bool Contains(T item) =>
            Container.Contains(item);

        public bool Exists(Predicate<T> match) =>
            Container.Exists(match);

        public int FindIndex(Predicate<T> match) =>
            Container.FindIndex(match);

        public int FindIndex(int startIndex, Predicate<T> match) =>
            Container.FindIndex(startIndex, match);

        public int FindIndex(int startIndex, int count, Predicate<T> match) =>
            Container.FindIndex(startIndex, match);

        public T FindLast(Predicate<T> match) =>
            Container.FindLast(match);

        public int FindLastIndex(Predicate<T> match) =>
            Container.FindIndex(match);

        public int FindLastIndex(int startIndex, Predicate<T> match) =>
            Container.FindLastIndex(startIndex, match);

        public int FindLastIndex(int startIndex, int count, Predicate<T> match) =>
            Container.FindLastIndex(startIndex, count, match);

        public List<T> GetRange(int index, int count) =>
            Container.GetRange(index, count);

        public int IndexOf(T item) =>
            Container.IndexOf(item);

        public int IndexOf(T item, int index) =>
            Container.IndexOf(item, index);

        public int IndexOf(T item, int index, int count) =>
            Container.IndexOf(item, index, count);

        public int LastIndexOf(T item) =>
            Container.LastIndexOf(item);

        public int LastIndexOf(T item, int index) =>
            Container.LastIndexOf(item, index);

        public int LastIndexOf(T item, int index, int count) =>
            Container.LastIndexOf(item, index, count);

        public void RemoveAt(int index)
        {
            Container.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            Container.RemoveRange(index, count);
        }

        public void Reverse()
        {
            Container.Reverse();
        }

        public void Reverse(int index, int count)
        {
            Container.Reverse(index, count);
        }

        public void Sort()
        {
            Container.Sort();
        }

        public void Sort(IComparer<T> comparer)
        {
            Container.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            Container.Sort(comparison);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Container.Sort(index, count, comparer);
        }

        public int Capacity
        {
            get
            {
                return Container.Capacity;
            }
            set
            {
                Container.Capacity = value;
            }
        }

        public int Count => Container.Count;

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

