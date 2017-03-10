namespace UIShell.OSGi.Loader
{
    using Dependency.Metadata;

    internal interface IDependencyBundleLoader : IRuntimeLoader
    {
        IDependentBundleConstraint Dependency { get; set; }
    }
}

