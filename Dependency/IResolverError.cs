namespace UIShell.OSGi.Dependency
{
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IResolverError
    {
        IBundleMetadata Bundle { get; set; }

        object Data { get; set; }

        ResolverErrorType Type { get; set; }

        IConstraint UnsatisfiedConstraint { get; set; }
    }
}

