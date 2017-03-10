namespace UIShell.OSGi.Dependency.Resolver
{
    using UIShell.OSGi;

    internal interface Interface0 : IResolvable, IConstraintNode
    {
        string BundleSymbolicName { get; }

        VersionRange BundleVersion { get; }
    }
}

