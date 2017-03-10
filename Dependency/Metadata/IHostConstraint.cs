namespace UIShell.OSGi.Dependency.Metadata
{
    using UIShell.OSGi;

    internal interface IHostConstraint : IConstraint
    {
        string BundleSymbolicName { get; set; }

        VersionRange BundleVersion { get; set; }
    }
}

