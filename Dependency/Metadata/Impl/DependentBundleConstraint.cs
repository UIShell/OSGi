namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;
    using UIShell.OSGi.Dependency.Metadata;

    internal class DependentBundleConstraint : Constraint, IConstraint, IDependentBundleConstraint
    {
        public override bool IsSatisfiedBy(IMetadata metadata)
        {
            Interface1 interface2 = metadata as Interface1;
            if (interface2 == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(interface2.SymbolicName))
            {
                return false;
            }
            if (interface2.SymbolicName.CompareTo(this.BundleSymbolicName) != 0)
            {
                return false;
            }
            if ((this.BundleVersion != null) && !this.BundleVersion.IsIncluded(interface2.Version))
            {
                return false;
            }
            base.DependentMetadata = metadata;
            return true;
        }

        public string BundleSymbolicName { get; set; }

        public VersionRange BundleVersion { get; set; }

        public ResolutionType Resolution { get; set; }
    }
}

