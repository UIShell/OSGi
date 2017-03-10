namespace UIShell.OSGi.Dependency.Metadata
{
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;

    internal interface IDependentAssemblyConstraint : IConstraint
    {
        string AssemblyName { get; set; }

        VersionRange AssemblyVersion { get; set; }

        string BundleSymbolicName { get; set; }

        VersionRange BundleVersion { get; set; }

        ResolutionType Resolution { get; set; }
    }
}

