namespace UIShell.OSGi.Utility
{
    using System;

    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T item)
        {
            Item = item;
        }

        public T Item { get; private set; }
    }
}

