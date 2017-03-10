namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi.Dependency.Metadata;

    internal class FragmentBundleMetadata : BundleMetadata, IMetadata, IBundleMetadata, IFragmentBundleMetadata
    {
        public IHostConstraint HostConstraint { get; set; }
    }
}

