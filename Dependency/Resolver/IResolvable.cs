namespace UIShell.OSGi.Dependency.Resolver
{
    using UIShell.OSGi.Dependency;

    internal interface IResolvable
    {
        bool Resolve();
        bool Unresolve();

        IResolver ConstraintResolver { get; }

        bool IsResolvable { get; set; }

        bool IsResolved { get; }
    }
}

