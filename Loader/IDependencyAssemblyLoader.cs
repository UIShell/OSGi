namespace UIShell.OSGi.Loader
{
    using Dependency.Metadata;

    internal interface IDependencyAssemblyLoader : IRuntimeLoader
    {
        IDependentAssemblyConstraint DependentAssembly { get; set; }
    }
}

