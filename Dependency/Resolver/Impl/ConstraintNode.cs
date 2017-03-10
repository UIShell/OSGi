namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;

    internal abstract class ConstraintNode : IResolvable, IConstraintNode, IDependentable
    {
        public ConstraintNode(IResolver resolver, Interface2 owner, IConstraint constraint)
        {
            this.ConstraintResolver = resolver;
            this.Owner = owner;
            this.Constraint = constraint;
        }

        public virtual bool IsSatisfiedBy(IMetadata metadata) => 
            this.Constraint.IsSatisfiedBy(metadata);

        public virtual bool Resolve()
        {
            if (this.IsResolved || !this.IsResolvable)
            {
                return this.IsResolved;
            }
            IMetadataNode node = this.ResolveNodeSource.Find(a => this.IsSatisfiedBy(a.Metadata));
            if (node != null)
            {
                this.DependentMetadataNode = node;
                IReferencable referencable = node as IReferencable;
                if (referencable != null)
                {
                    referencable.AddReferencedConstraint(this);
                }
                this.IsResolved = true;
            }
            else
            {
                this.IsResolvable = false;
            }
            return this.IsResolvable;
        }

        public virtual bool Unresolve()
        {
            IReferencable dependentMetadataNode = this.DependentMetadataNode as IReferencable;
            if (dependentMetadataNode != null)
            {
                dependentMetadataNode.RemoveReferencedConstraint(this);
            }
            this.DependentMetadataNode = null;
            this.IsResolved = false;
            return true;
        }

        public IConstraint Constraint { get; set; }

        public IResolver ConstraintResolver { get; set; }

        public IMetadataNode DependentMetadataNode { get; set; }

        public bool IsResolvable { get; set; }

        public bool IsResolved { get; protected set; }

        public Interface2 Owner { get; set; }

        protected abstract List<IMetadataNode> ResolveNodeSource { get; }
    }
}

