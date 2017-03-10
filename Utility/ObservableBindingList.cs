namespace UIShell.OSGi.Utility
{
    using System;
    using System.ComponentModel;

    public class ObservableBindingList<T> : BindingList<T>
    {
        public event EventHandler<EventArgs<T>> Removed;

        public event ListChangedEventHandler Removing;

        protected void OnRemoved(EventArgs<T> e)
        {
            if (Removed != null)
            {
                Removed(this, e);
            }
        }

        protected void OnRemoving(ListChangedEventArgs e)
        {
            if (Removing != null)
            {
                Removing(this, e);
            }
        }

        protected override void RemoveItem(int index)
        {
            if ((index > -1) && (index < base.Count))
            {
                OnRemoving(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
                T item = base[index];
                base.RemoveItem(index);
                OnRemoved(new EventArgs<T>(item));
            }
        }
    }
}

