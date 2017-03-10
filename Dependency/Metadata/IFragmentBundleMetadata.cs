namespace UIShell.OSGi.Dependency.Metadata
{
    internal interface IFragmentBundleMetadata : IMetadata, IBundleMetadata
    {
        IHostConstraint HostConstraint { get; set; }
    }
}

