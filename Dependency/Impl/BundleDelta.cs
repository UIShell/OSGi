namespace UIShell.OSGi.Dependency.Impl
{
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Utility;

    internal class BundleDelta : IBundleDelta
    {
        public BundleDelta(IBundleMetadata bundleMetadata, BundleDeltaType deltaType)
        {
            AssertUtility.ArgumentNotNull(bundleMetadata, "bundleMetadata");
            AssertUtility.EnumDefined(typeof(BundleDeltaType), deltaType, "deltaType");
            this.Bundle = bundleMetadata;
            this.Type = deltaType;
        }

        public IBundleMetadata Bundle { get; set; }

        public BundleDeltaType Type { get; set; }
    }
}

