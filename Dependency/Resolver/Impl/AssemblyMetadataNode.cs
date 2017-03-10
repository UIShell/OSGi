namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;

    internal class AssemblyMetadataNode : MetadataNode, IMetadataNode, IReferencable, IAssemblyMetadataNode
    {
        private Referencable _referencableProxy;

        public AssemblyMetadataNode(IAssemblyMetadata metadata) : this(metadata, null)
        {
        }

        public AssemblyMetadataNode(IAssemblyMetadata metadata, Interface2 owner) : base(metadata, owner)
        {
            this._referencableProxy = new Referencable();
            this.AssemblyName = metadata.AssemblyName;
        }

        public void AddReferencedConstraint(IConstraintNode constraintNode)
        {
            this._referencableProxy.AddReferencedConstraint(constraintNode);
        }

        public void RemoveReferencedConstraint(IConstraintNode constraintNode)
        {
            this._referencableProxy.RemoveReferencedConstraint(constraintNode);
        }

        public System.Reflection.AssemblyName AssemblyName { get; set; }

        public List<IConstraintNode> ReferencedConstraints =>
            this._referencableProxy.ReferencedConstraints;
    }
}

