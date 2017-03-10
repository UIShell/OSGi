namespace UIShell.OSGi.Dependency.Resolver
{
    internal interface IFragmentBundleMetadataNode : IResolvable, IMetadataNode, Interface2
    {
        Interface0 HostConstraintNode { get; }

        IHostBundleMetadataNode HostNode { get; set; }
    }
}

