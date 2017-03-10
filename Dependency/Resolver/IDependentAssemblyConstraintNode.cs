namespace UIShell.OSGi.Dependency.Resolver
{
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;

    internal interface IDependentAssemblyConstraintNode : IResolvable, IConstraintNode, IDependentable
    {
        string AssemblyName { get; }

        VersionRange AssemblyVersion { get; }

        string BundleSymbolicName { get; }

        VersionRange BundleVersion { get; }

        ResolutionType Resolution { get; }
    }
}

