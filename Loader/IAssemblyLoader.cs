namespace UIShell.OSGi.Loader
{
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IAssemblyLoader : IRuntimeLoader
    {
        IAssemblyMetadata Import { get; }
    }
}

