namespace UIShell.OSGi.Core.Service
{
    using OSGi;
    using Configuration.BundleManifest;

    public interface IBundleFactory
    {
        IBundle CreateBundle(BundleData bundleData);

        int InitialBundleId { get; }

        int MaxBundleID { get; }
    }
}

