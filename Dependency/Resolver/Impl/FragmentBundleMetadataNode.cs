namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;

    internal class FragmentBundleMetadataNode : BundleMetadataNode, IResolvable, IMetadataNode, Interface2, IFragmentBundleMetadataNode
    {
        public FragmentBundleMetadataNode(IResolver resolver, IFragmentBundleMetadata metadata) : base(resolver, metadata)
        {
            this.HostConstraintNode = new UIShell.OSGi.Dependency.Resolver.Impl.HostConstraintNode(resolver, this, metadata.HostConstraint);
        }

        public Interface0 HostConstraintNode { get; private set; }

        public IHostBundleMetadataNode HostNode { get; set; }
    }
}

