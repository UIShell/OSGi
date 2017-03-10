namespace UIShell.OSGi.Core.Service
{
    using OSGi;
    using Configuration.BundleManifest;

    public class BundleInfo
    {
        public BundleData BundleData { get; set; }

        public BundleState BundleState { get; set; }
    }
}

