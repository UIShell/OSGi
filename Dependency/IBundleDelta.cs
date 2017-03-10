namespace UIShell.OSGi.Dependency
{
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IBundleDelta
    {
        IBundleMetadata Bundle { get; set; }

        BundleDeltaType Type { get; set; }
    }
}

