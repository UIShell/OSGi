namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency.Resolver;

    internal class Referencable : IReferencable
    {
        public Referencable()
        {
            this.ReferencedConstraints = new List<IConstraintNode>();
        }

        public void AddReferencedConstraint(IConstraintNode constraintNode)
        {
            if (!this.ReferencedConstraints.Contains(constraintNode))
            {
                this.ReferencedConstraints.Add(constraintNode);
            }
        }

        public void RemoveReferencedConstraint(IConstraintNode constraintNode)
        {
            this.ReferencedConstraints.Remove(constraintNode);
        }

        public List<IConstraintNode> ReferencedConstraints { get; private set; }
    }
}

