namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi.Dependency.Metadata;

    internal class Metadata : IMetadata
    {
        public IBundleMetadata Owner { get; set; }

        public System.Version Version { get; set; }
    }
}

