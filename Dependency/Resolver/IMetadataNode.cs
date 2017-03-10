namespace UIShell.OSGi.Dependency.Resolver
{
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IMetadataNode
    {
        IMetadata Metadata { get; set; }

        Interface2 Owner { get; set; }

        System.Version Version { get; }
    }
}

