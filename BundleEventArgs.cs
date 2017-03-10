namespace UIShell.OSGi
{
    using System;

    public abstract class BundleEventArgs : EventArgs
    {
        internal BundleEventArgs(IBundle bundle)
        {
            if (bundle == null)
            {
                throw new ArgumentNullException();
            }
            Bundle = bundle;
        }

        public IBundle Bundle { get; private set; }
    }
}

