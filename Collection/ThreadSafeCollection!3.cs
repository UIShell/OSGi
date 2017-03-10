namespace UIShell.OSGi.Collection
{
    using System;
    using System.Threading;

    public abstract class ThreadSafeCollection<T, ContainterType, LockerType> where LockerType: ContainerLocker<ContainterType>
    {
        private int _millisecondsTimeoutOnLock;
        private object _syncRoot;

        public ThreadSafeCollection(int minsecondsTimeoutOnLock)
        {
            this.MillisecondsTimeoutOnLock = minsecondsTimeoutOnLock;
        }

        protected abstract LockerType CreateLocker();
        public LockerType Lock() => 
            this.CreateLocker();

        public void Lock(Action<LockerType> callback)
        {
            if (callback != null)
            {
                using (LockerType local = this.CreateLocker())
                {
                    callback(local);
                }
            }
        }

        protected ContainterType Container { get; set; }

        public int MillisecondsTimeoutOnLock
        {
            get => 
                this._millisecondsTimeoutOnLock;
            set
            {
                if (value < 0)
                {
                    this._millisecondsTimeoutOnLock = 0;
                }
                this._millisecondsTimeoutOnLock = value;
            }
        }

        protected virtual object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }
    }
}

