namespace UIShell.OSGi.Utility
{
    using System;
    using System.Collections.Generic;

    internal sealed class ListUtility
    {
        private ListUtility()
        {
        }

        public static bool ArrayContains<T>(T[] parent, T[] child) where T: IComparable
        {
            if (parent == null)
            {
                return false;
            }
            if (child != null)
            {
                for (int i = 0; i < child.Length; i++)
                {
                    if (!ArrayContains<T>(parent, child[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ArrayContains<T>(T[] array, T item) where T: IComparable
        {
            if ((array != null) && (array.Length != 0))
            {
                if (item == null)
                {
                    return true;
                }
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].CompareTo(item) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static T[] CombineCollection<T>(T[] existing, T newOne)
        {
            List<T> list = null;
            if (existing == null)
            {
                list = new List<T>(new T[] { newOne });
            }
            else
            {
                list = new List<T>(existing) {
                    newOne
                };
            }
            return list.ToArray();
        }

        public static List<T> RemoveAll<T>(ICollection<T> source, Func<T, bool> comparer)
        {
            List<T> list = new List<T>(source);
            List<T> removeItems = new List<T>();
            list.ForEach(delegate (T item) {
                if (comparer(item))
                {
                    source.Remove(item);
                    removeItems.Add(item);
                }
            });
            return removeItems;
        }

        public static void Union<T>(List<T> listA, IEnumerable<T> listB)
        {
            if (listA == null)
            {
                throw new ArgumentNullException();
            }
            if (listB != null)
            {
                List<T> collection = new List<T>();
                foreach (T local in listB)
                {
                    if (!listA.Contains(local))
                    {
                        collection.Add(local);
                    }
                }
                listA.AddRange(collection);
            }
        }
    }
}

