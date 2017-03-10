namespace UIShell.OSGi.Dependency.Metadata
{
    internal interface IConstraint
    {
        bool IsSatisfiedBy(IMetadata metadata);
        void Reset();

        IMetadata DependentMetadata { get; set; }

        bool IsResolved { get; }

        IBundleMetadata Owner { get; set; }
    }
}

