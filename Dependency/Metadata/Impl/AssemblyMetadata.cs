namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi.Dependency.Metadata;

    internal class AssemblyMetadata : Metadata, IMetadata, IAssemblyMetadata
    {
        public System.Reflection.AssemblyName AssemblyName { get; set; }

        public bool IsDuplicatedWithGlobalAssembly { get; set; }

        public bool MultipleVersions { get; set; }

        public string Path { get; set; }

        public bool Share { get; set; }
    }
}

