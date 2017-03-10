namespace UIShell.OSGi.Dependency.Resolver
{
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;

    internal interface IDependentBundleConstraintNode : IResolvable, IConstraintNode, IDependentable
    {
        string BundleSymbolicName { get; }

        VersionRange BundleVersion { get; }

        ResolutionType Resolution { get; }
    }
}

