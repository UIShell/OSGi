namespace UIShell.OSGi.Dependency.Metadata
{
    using UIShell.OSGi.Configuration.BundleManifest;

    internal interface IMetadataBuilder
    {
        IBundleMetadata BuildBundleMetadata(BundleData bundleData, long bundleID);
    }
}

