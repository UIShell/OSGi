namespace UIShell.OSGi
{
    public class BundleLazyActivatedEventArgs : BundleEventArgs
    {
        internal BundleLazyActivatedEventArgs(IBundle bundle)
            : base(bundle)
        {
        }
    }
}

