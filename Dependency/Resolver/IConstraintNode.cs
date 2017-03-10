namespace UIShell.OSGi.Dependency.Resolver
{
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IConstraintNode : IResolvable
    {
        bool IsSatisfiedBy(IMetadata metadata);

        IConstraint Constraint { get; set; }

        Interface2 Owner { get; set; }
    }
}

