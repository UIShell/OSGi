namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using UIShell.OSGi.Dependency.Metadata;

    internal abstract class Constraint : IConstraint
    {
        protected Constraint()
        {
        }

        public abstract bool IsSatisfiedBy(IMetadata metadata);
        public virtual void Reset()
        {
            this.DependentMetadata = null;
        }

        public IMetadata DependentMetadata { get; set; }

        public bool IsResolved =>
            (this.DependentMetadata != null);

        public IBundleMetadata Owner { get; set; }

        public UIShell.OSGi.VersionRange VersionRange { get; set; }
    }
}

