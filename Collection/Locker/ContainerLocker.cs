namespace UIShell.OSGi.Collection.Locker
{
    using System;
    using Collection;

    public class ContainerLocker<ContainerType> : DisposableLocker
    {
        public ContainerLocker(object syncRoot, ContainerType container, int millisecondsTimeout) : base(syncRoot, millisecondsTimeout)
        {
            if (syncRoot == null)
            {
                throw new ArgumentNullException("mutex");
            }
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            Container = container;
        }

        public void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        protected ContainerType Container { get; private set; }
    }
}

