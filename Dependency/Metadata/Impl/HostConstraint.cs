namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi;
    using UIShell.OSGi.Dependency.Metadata;

    internal class HostConstraint : Constraint, IConstraint, IHostConstraint
    {
        public override bool IsSatisfiedBy(IMetadata metadata)
        {
            if ((metadata == null) || !(metadata is Interface1))
            {
                return false;
            }
            Interface1 interface2 = metadata as Interface1;
            if (string.IsNullOrEmpty(interface2.SymbolicName))
            {
                return false;
            }
            if (interface2.SymbolicName.CompareTo(this.BundleSymbolicName) != 0)
            {
                return false;
            }
            if (this.BundleVersion != null)
            {
                return this.BundleVersion.IsIncluded(interface2.Version);
            }
            return true;
        }

        public string BundleSymbolicName { get; set; }

        public VersionRange BundleVersion { get; set; }
    }
}

