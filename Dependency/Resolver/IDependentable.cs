namespace UIShell.OSGi.Dependency.Resolver
{
    internal interface IDependentable
    {
        IMetadataNode DependentMetadataNode { get; }
    }
}

