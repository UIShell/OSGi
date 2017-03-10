namespace UIShell.OSGi.Dependency.Metadata
{
    internal interface IAssemblyMetadata : IMetadata
    {
        System.Reflection.AssemblyName AssemblyName { get; set; }

        bool IsDuplicatedWithGlobalAssembly { get; set; }

        bool MultipleVersions { get; set; }

        string Path { get; set; }

        bool Share { get; set; }
    }
}

