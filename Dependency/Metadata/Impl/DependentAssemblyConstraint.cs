namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;
    using UIShell.OSGi.Dependency.Metadata;

    internal class DependentAssemblyConstraint : Constraint, IConstraint, IDependentAssemblyConstraint
    {
        private bool IsSatisfiedBy(IAssemblyMetadata dependentAssembly)
        {
            if (string.IsNullOrEmpty(dependentAssembly.Owner.SymbolicName))
            {
                return false;
            }
            if (dependentAssembly.Owner.SymbolicName.CompareTo(this.BundleSymbolicName) != 0)
            {
                return false;
            }
            if (dependentAssembly.AssemblyName.Name != this.AssemblyName)
            {
                return false;
            }
            if ((this.BundleVersion != null) && !this.BundleVersion.IsIncluded(dependentAssembly.Owner.Version))
            {
                return false;
            }
            if ((this.AssemblyVersion != null) && !this.AssemblyVersion.IsIncluded(dependentAssembly.Version))
            {
                return false;
            }
            base.DependentMetadata = dependentAssembly;
            return true;
        }

        public override bool IsSatisfiedBy(IMetadata metadata)
        {
            IAssemblyMetadata dependentAssembly = metadata as IAssemblyMetadata;
            if (dependentAssembly == null)
            {
                return false;
            }
            return this.IsSatisfiedBy(dependentAssembly);
        }

        public string AssemblyName { get; set; }

        public VersionRange AssemblyVersion { get; set; }

        public string BundleSymbolicName { get; set; }

        public VersionRange BundleVersion { get; set; }

        public ResolutionType Resolution { get; set; }
    }
}

