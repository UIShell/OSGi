namespace UIShell.OSGi.Dependency.Metadata
{
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;

    internal interface IDependentBundleConstraint : IConstraint
    {
        string BundleSymbolicName { get; set; }

        VersionRange BundleVersion { get; set; }

        ResolutionType Resolution { get; set; }
    }
}

