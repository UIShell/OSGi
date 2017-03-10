namespace UIShell.OSGi.Dependency.Resolver
{
    using System.Collections.Generic;

    internal interface IReferencable
    {
        void AddReferencedConstraint(IConstraintNode constraintNode);
        void RemoveReferencedConstraint(IConstraintNode constraintNode);

        List<IConstraintNode> ReferencedConstraints { get; }
    }
}

