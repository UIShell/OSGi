namespace UIShell.OSGi.Collection.Locker
{
    using System.Collections;
    using System.Collections.Generic;

    public class EnumerableLocker<T, ContainterType> : ContainerLocker<ContainterType>,
        IEnumerable<T>, IEnumerable where ContainterType: IEnumerable<T>
    {
        public EnumerableLocker(object syncRoot, ContainterType container, int millisecondsTimeout)
            : base(syncRoot, container, millisecondsTimeout)
        {
        }

        public IEnumerator<T> GetEnumerator() => Container.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Container.GetEnumerator();
    }
}

