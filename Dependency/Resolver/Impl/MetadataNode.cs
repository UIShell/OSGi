namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;

    internal class MetadataNode : IMetadataNode
    {
        public MetadataNode() : this(null)
        {
        }

        public MetadataNode(IMetadata metadata) : this(metadata, null)
        {
        }

        public MetadataNode(IMetadata metadata, Interface2 owner)
        {
            this.Metadata = metadata;
            this.Owner = owner;
            this.Version = metadata.Version;
        }

        public IMetadata Metadata { get; set; }

        public Interface2 Owner { get; set; }

        public System.Version Version { get; private set; }
    }
}

