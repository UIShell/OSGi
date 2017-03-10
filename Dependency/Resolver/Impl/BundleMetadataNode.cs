namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;
    using UIShell.OSGi.Utility;

    [DebuggerDisplay("{SymbolicName}:IsResolved:{IsResolved},IsResolvable:{IsResolvable}")]
    internal class BundleMetadataNode : MetadataNode, IResolvable, IMetadataNode, Interface2
    {
        public BundleMetadataNode(IResolver resolver, IBundleMetadata metadata) : base(metadata)
        {
            Action<IAssemblyMetadata> action = null;
            Action<IAssemblyMetadata> action2 = null;
            Action<IDependentBundleConstraint> action3 = null;
            Action<IDependentAssemblyConstraint> action4 = null;
            this.ConstraintResolver = resolver;
            this.BundleID = metadata.BundleID;
            this.SymbolicName = metadata.SymbolicName;
            this.DependentAssemblyNodes = new List<IDependentAssemblyConstraintNode>(metadata.DependentAssemblies.Count);
            this.DependentBundleNodes = new List<IDependentBundleConstraintNode>(metadata.DependentBundles.Count);
            this.SharedAssemblyNodes = new List<IAssemblyMetadataNode>(metadata.SharedAssemblies.Count);
            this.PrivateAssemblyNodes = new List<IAssemblyMetadataNode>(metadata.PrivateAssemblies.Count);
            action = item => this.SharedAssemblyNodes.Add(new AssemblyMetadataNode(item, this));
            metadata.SharedAssemblies.ForEach(action);
            action2 = item => this.PrivateAssemblyNodes.Add(new AssemblyMetadataNode(item, this));
            metadata.PrivateAssemblies.ForEach(action2);
            action3 = item => this.DependentBundleNodes.Add(new DependentBundleConstraintNode(resolver, this, item));
            metadata.DependentBundles.ForEach(action3);
            action4 = item => this.DependentAssemblyNodes.Add(new DependentAssemblyConstraintNode(resolver, this, item));
            metadata.DependentAssemblies.ForEach(action4);
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public bool Resolve()
        {
            if (!this.IsResolved && this.IsResolvable)
            {
                IDependentAssemblyConstraintNode node2 = this.DependentAssemblyNodes.Find(a => !a.Resolve());
                if (node2 != null)
                {
                    FileLogUtility.Error(string.Format(Messages.BundleDependentAssemblyNotResolved, new object[] { node2.Owner.SymbolicName, node2.Owner.Version, node2.AssemblyName, node2.AssemblyVersion, node2.BundleSymbolicName, node2.BundleVersion }));
                    this.IsResolvable = false;
                    this.IsResolved = false;
                    return this.IsResolvable;
                }
                IDependentBundleConstraintNode node = this.DependentBundleNodes.Find(a => !a.Resolve());
                if (node != null)
                {
                    this.IsResolvable = false;
                    this.IsResolved = false;
                    FileLogUtility.Error(string.Format(Messages.BundleDependentBundleNotResolved, new object[] { node.Owner.SymbolicName, node.Owner.Version, node.BundleSymbolicName, node.BundleVersion }));
                }
                else
                {
                    this.IsResolved = true;
                    this.ConstraintResolver.ResolvedAssemblyMetadataNodes.AddRange(this.SharedAssemblyNodes);
                    this.ConstraintResolver.ResolvedAssemblyMetadataNodes.AddRange(this.PrivateAssemblyNodes);
                }
                return this.IsResolvable;
            }
            Action<IAssemblyMetadata> action = null;
            IBundleMetadata bundleMetadata = base.Metadata as IBundleMetadata;
            if (bundleMetadata.AssembliesFailedToLoad.Count > 0)
            {
                if (action == null)
                {
                    action = delegate (IAssemblyMetadata node) {
                        FileLogUtility.Error(string.Format(Messages.BundleLocalAssemblyResolvedFailed, bundleMetadata.SymbolicName, bundleMetadata.Version, node.Path));
                    };
                }
                bundleMetadata.AssembliesFailedToLoad.ForEach(action);
            }
            return this.IsResolved;
        }

        public virtual bool Unresolve()
        {
            this.ConstraintResolver.ResolvedAssemblyMetadataNodes.RemoveAll(a => this.SharedAssemblyNodes.Contains(a));
            this.ConstraintResolver.ResolvedAssemblyMetadataNodes.RemoveAll(a => this.PrivateAssemblyNodes.Contains(a));
            this.IsResolved = false;
            return true;
        }

        public long BundleID { get; private set; }

        public IResolver ConstraintResolver { get; private set; }

        public List<IDependentAssemblyConstraintNode> DependentAssemblyNodes { get; private set; }

        public List<IDependentBundleConstraintNode> DependentBundleNodes { get; private set; }

        public bool IsResolvable { get; set; }

        public bool IsResolved
        {
            get => 
                ((IBundleMetadata) base.Metadata).IsResolved;
            protected set
            {
                ((IBundleMetadata) base.Metadata).IsResolved = value;
            }
        }

        public List<IAssemblyMetadataNode> PrivateAssemblyNodes { get; private set; }

        public List<IAssemblyMetadataNode> SharedAssemblyNodes { get; private set; }

        public string SymbolicName { get; private set; }
    }
}

