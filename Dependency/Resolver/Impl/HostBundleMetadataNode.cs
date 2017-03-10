namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System;
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;
    using UIShell.OSGi.Utility;

    internal class HostBundleMetadataNode : BundleMetadataNode, IResolvable, IMetadataNode, Interface2, IReferencable, IHostBundleMetadataNode
    {
        private Referencable _referencableProxy;

        public HostBundleMetadataNode(IResolver resolver, Interface1 metadata) : base(resolver, metadata)
        {
            this._referencableProxy = new Referencable();
            AssertUtility.ArgumentNotNull(resolver, "resolver");
            this.FragmentNodes = new List<IFragmentBundleMetadataNode>();
            BundleUtility.BuildFragments(this);
        }

        public void AddReferencedConstraint(IConstraintNode constraintNode)
        {
            this._referencableProxy.AddReferencedConstraint(constraintNode);
        }

        public List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>> AttachAllFragments()
        {
            List<IFragmentBundleMetadata> metadatas = BundleUtility.GetMetadatas<IFragmentBundleMetadata>(base.ConstraintResolver.State.Bundles);
            BundleUtility.BindFragmentMetadatas((Interface1) base.Metadata, metadatas);
            List<Interface2> unResolverNodes = base.ConstraintResolver.State.Resolver.UnResolverNodes;
            BundleUtility.BuildFragments(this);
            List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>> rt = new List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>>(this.FragmentNodes.Count);
            this.FragmentNodes.ForEach(delegate (IFragmentBundleMetadataNode a) {
                base.ConstraintResolver.ResolvedAssemblyMetadataNodes.AddRange(a.SharedAssemblyNodes);
                base.ConstraintResolver.ResolvedAssemblyMetadataNodes.AddRange(a.PrivateAssemblyNodes);
            });
            this.FragmentNodes.ForEach(delegate (IFragmentBundleMetadataNode fragementNode) {
                rt.Add(TupleUtility.CreateTuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>(fragementNode, new List<IAssemblyMetadataNode>(fragementNode.SharedAssemblyNodes)));
            });
            return rt;
        }

        public List<IAssemblyMetadataNode> AttachFragment(IFragmentBundleMetadataNode fragment)
        {
            AssertUtility.ArgumentNotNull(fragment, "fragement");
            if (!this.FragmentNodes.Contains(fragment))
            {
                fragment.HostNode = this;
                base.ConstraintResolver.ResolvedAssemblyMetadataNodes.AddRange(fragment.SharedAssemblyNodes);
                base.ConstraintResolver.ResolvedAssemblyMetadataNodes.AddRange(fragment.PrivateAssemblyNodes);
                this.FragmentNodes.Add(fragment);
                this.SortByBundleID<IFragmentBundleMetadataNode>(this.FragmentNodes, true);
            }
            return null;
        }

        public List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>> DetachAllFragments()
        {
            List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>> rt = new List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>>();
            new List<IFragmentBundleMetadataNode>(this.FragmentNodes).ForEach(delegate (IFragmentBundleMetadataNode a) {
                rt.Add(new Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>(a, this.DetachFagment(a)));
            });
            return null;
        }

        public List<IAssemblyMetadataNode> DetachFagment(IFragmentBundleMetadataNode fragment)
        {
            if (fragment.HostNode != this)
            {
                throw new InvalidOperationException("A framgent node can only be detached from it's host node");
            }
            fragment.HostNode = null;
            this.FragmentNodes.Remove(fragment);
            base.ConstraintResolver.ResolvedAssemblyMetadataNodes.RemoveAll(a => fragment.SharedAssemblyNodes.Contains(a));
            return new List<IAssemblyMetadataNode>(fragment.SharedAssemblyNodes);
        }

        public bool IsReferenced() => 
            (this.ReferencedConstraints.Find(a => !(a.Owner is IFragmentBundleMetadataNode)) != null);

        public void RemoveReferencedConstraint(IConstraintNode constraintNode)
        {
            this._referencableProxy.RemoveReferencedConstraint(constraintNode);
        }

        private void SortByBundleID<T>(List<T> collection, bool smallFirst) where T: Interface2
        {
            collection.Sort(delegate (T x, T y) {
                if (smallFirst)
                {
                    return ((IBundleMetadata) x.Metadata).BundleID.CompareTo(((IBundleMetadata) y.Metadata).BundleID);
                }
                return ((IBundleMetadata) y.Metadata).BundleID.CompareTo(((IBundleMetadata) x.Metadata).BundleID);
            });
        }

        public override string ToString()
        {
            Interface1 metadata = base.Metadata as Interface1;
            if (metadata == null)
            {
                return $"{base.SymbolicName},Version:{base.Version}";
            }
            return $"{base.SymbolicName},Version:{base.Version},Location:{metadata.Location}";
        }

        public override bool Unresolve()
        {
            List<IConstraintNode> list = new List<IConstraintNode>(this.ReferencedConstraints);
            if (list.Find(a => a.Unresolve()) == null)
            {
                base.IsResolved = false;
            }
            return base.Unresolve();
        }

        public List<IFragmentBundleMetadataNode> FragmentNodes { get; private set; }

        public List<IConstraintNode> ReferencedConstraints =>
            this._referencableProxy.ReferencedConstraints;
    }
}

