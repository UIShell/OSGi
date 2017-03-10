namespace UIShell.OSGi.Collection.Locker
{
    using System.Collections.Generic;

    public class DictionaryLocker<TKey, TValue> : EnumerableLocker<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>>
    {
        public DictionaryLocker(object mutex, Dictionary<TKey, TValue> list, int millisecondsTimeout)
            : base(mutex, list, millisecondsTimeout)
        {
        }

        public void Add(TKey key, TValue value)
        {
            Container.Add(key, value);
        }

        public bool ContainsKey(TKey key) =>
            Container.ContainsKey(key);

        public bool ContainsValue(TValue value) =>
            Container.ContainsValue(value);

        public bool TryGetValue(TKey key, out TValue value) =>
            Container.TryGetValue(key, out value);

        public int Count => Container.Count;

        public TValue this[TKey key]
        {
            get
            {
                return Container[key];
            }
            set
            {
                Container[key] = value;
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => Container.Keys;

        public Dictionary<TKey, TValue>.ValueCollection Values => Container.Values;
    }
}

