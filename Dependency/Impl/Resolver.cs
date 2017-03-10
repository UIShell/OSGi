namespace UIShell.OSGi.Dependency.Impl
{
    using System;
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;
    using UIShell.OSGi.Dependency.Resolver.Impl;
    using UIShell.OSGi.Utility;

    internal class Resolver : IResolver
    {
        public event EventHandler<BundleMetadataEventArgs> BundleResolved;

        public event EventHandler<BundleMetadataEventArgs> BundleResolving;

        public event EventHandler<BundleMetadataEventArgs> BundleUnresolved;

        public event EventHandler<BundleMetadataEventArgs> BundleUnresolving;

        public Resolver(IState state)
        {
            this.State = state;
            this.State.BundleAdded += new EventHandler<BundleMetadataEventArgs>(this.State_BundleAdded);
            this.State.BundleRemoved += new EventHandler<BundleMetadataEventArgs>(this.State_BundleRemoved);
            this.UnResolverNodes = new List<Interface2>();
            this.ResolvedNodes = new List<Interface2>();
            this.ResolvedAssemblyMetadataNodes = new List<IAssemblyMetadataNode>();
        }

        private List<Interface2> GetDependentBundles(List<Interface2> bundles, Interface2 bundleNode)
        {
            List<Interface2> dependentNodes = new List<Interface2>();
            bundleNode.DependentBundleNodes.ForEach(delegate (IDependentBundleConstraintNode dependentConstraint) {
                Interface2 item = bundles.Find(b => ((IBundleMetadata) b.Metadata).SymbolicName == dependentConstraint.BundleSymbolicName);
                if (item != null)
                {
                    dependentNodes.Add(item);
                }
            });
            return dependentNodes;
        }

        private List<Interface2> GetReferencedBundles(List<Interface2> bundles, Interface2 bundleNode) => 
            bundles.FindAll(a => a.DependentBundleNodes.Find(dependent => dependent.BundleSymbolicName == bundleNode.SymbolicName) != null);

        private void LogAssembliesFailedToLoaded(IBundleMetadata unresolvedNode)
        {
            Action<IAssemblyMetadata> action = null;
            if (unresolvedNode.AssembliesFailedToLoad.Count > 0)
            {
                if (action == null)
                {
                    action = node => FileLogUtility.Error(string.Format(Messages.BundleLocalAssemblyResolvedFailed, unresolvedNode.SymbolicName, unresolvedNode.Version, node.Path));
                }
                unresolvedNode.AssembliesFailedToLoad.ForEach(action);
            }
        }

        private void LogUnresolvedNode(IFragmentBundleMetadataNode unresolvedNode)
        {
            FileLogUtility.Inform(string.Format(Messages.FragmentDetachedForHostNotResolved, new object[] { unresolvedNode.SymbolicName, unresolvedNode.Version, unresolvedNode.HostNode.SymbolicName, unresolvedNode.HostNode.Version }));
        }

        private void LogUnresolvedNode(Interface2 unresolvedNode)
        {
            FileLogUtility.Inform(string.Format(Messages.BundleNotResolved, unresolvedNode.SymbolicName, unresolvedNode.Version));
        }

        private void LogUnresolvedNode(Interface2 unresolvedNode, Interface2 dependentNode)
        {
            FileLogUtility.Inform(string.Format(Messages.BundleMarkedUnresolved, new object[] { unresolvedNode.SymbolicName, unresolvedNode.Version, dependentNode.SymbolicName, dependentNode.Version }));
        }

        private void MarkAsResolvable(Interface2 bundle)
        {
            IBundleMetadata unresolvedNode = bundle.Metadata as IBundleMetadata;
            bool resovable = (unresolvedNode != null) && (unresolvedNode.AssembliesFailedToLoad.Count == 0);
            if (!resovable)
            {
                this.LogAssembliesFailedToLoaded(unresolvedNode);
            }
            bundle.IsResolvable = resovable;
            bundle.DependentAssemblyNodes.ForEach((Action<IDependentAssemblyConstraintNode>) (dependent => (dependent.IsResolvable = resovable)));
            bundle.DependentBundleNodes.ForEach((Action<IDependentBundleConstraintNode>) (dependent => (dependent.IsResolvable = resovable)));
        }

        private void MarkUnResolvable(List<Interface2> bundles, params Interface2[] unresolvedNodes)
        {
            Action<Interface2> action = null;
            foreach (Interface2 unresolvedNode in unresolvedNodes)
            {
                this.LogUnresolvedNode(unresolvedNode);
                List<Interface2> referencedBundles = this.GetReferencedBundles(bundles, unresolvedNode);
                referencedBundles.RemoveAll(referencedNode => !referencedNode.IsResolvable);
                if (action == null)
                {
                    action = delegate (Interface2 referencedNode) {
                        referencedNode.IsResolvable = false;
                        this.LogUnresolvedNode(referencedNode, unresolvedNode);
                    };
                }
                referencedBundles.ForEach(action);
                IFragmentBundleMetadataNode fragment = unresolvedNode as IFragmentBundleMetadataNode;
                if ((fragment != null) && (fragment.HostNode != null))
                {
                    fragment.HostNode.DetachFagment(fragment);
                    this.LogUnresolvedNode(fragment);
                }
                this.MarkUnResolvable(bundles, referencedBundles.ToArray());
            }
        }

        protected virtual void OnBundleResolved(BundleMetadataEventArgs e)
        {
            if (this.BundleResolved != null)
            {
                this.BundleResolved(this, e);
            }
        }

        protected virtual void OnBundleResolving(BundleMetadataEventArgs e)
        {
            if (this.BundleResolving != null)
            {
                this.BundleResolving(this, e);
            }
        }

        protected virtual void OnBundleUnresolved(BundleMetadataEventArgs e)
        {
            if (this.BundleUnresolved != null)
            {
                this.BundleUnresolved(this, e);
            }
        }

        protected virtual void OnBundleUnresolving(BundleMetadataEventArgs e)
        {
            if (this.BundleUnresolving != null)
            {
                this.BundleUnresolving(this, e);
            }
        }

        public void Resolve(List<IBundleMetadata> unResolvedMetadatas)
        {
            if (this.State != null)
            {
                List<Interface2> needUnResolvedNodes = this.ResolvedNodes.FindAll(a => unResolvedMetadatas.Contains((IBundleMetadata) a.Metadata));
                this.TryUnResolveBundles(needUnResolvedNodes);
                List<Interface2> bundles = this.UnResolverNodes.FindAll(a => unResolvedMetadatas.Contains((IBundleMetadata) a.Metadata));
                bundles.ForEach(delegate (object a) {
                    IHostBundleMetadataNode node = a as IHostBundleMetadataNode;
                    if (node != null)
                    {
                        node.AttachAllFragments();
                    }
                });
                this.TryResolveBundles(bundles);
            }
        }

        private void State_BundleAdded(object sender, BundleMetadataEventArgs e)
        {
            Interface1 metadata = e.Metadata as Interface1;
            if (metadata != null)
            {
                this.UnResolverNodes.Add(new HostBundleMetadataNode(this, metadata));
            }
            else
            {
                this.UnResolverNodes.Add(new FragmentBundleMetadataNode(this, (IFragmentBundleMetadata) e.Metadata));
            }
        }

        private void State_BundleRemoved(object sender, BundleMetadataEventArgs e)
        {
            Predicate<Interface2> match = null;
            Predicate<Interface2> predicate2 = null;
            Predicate<Interface2> predicate3 = null;
            Interface1 host = e.Metadata as Interface1;
            if (host != null)
            {
                if (host.IsResolved)
                {
                    if (match == null)
                    {
                        match = a => a.Metadata == host;
                    }
                    Interface2 interface3 = this.ResolvedNodes.Find(match);
                    if (interface3 != null)
                    {
                        interface3.Unresolve();
                    }
                    if (predicate2 == null)
                    {
                        predicate2 = a => a.Metadata == e.Metadata;
                    }
                    this.ResolvedNodes.RemoveAll(predicate2);
                }
                else
                {
                    if (predicate3 == null)
                    {
                        predicate3 = a => a.Metadata == e.Metadata;
                    }
                    this.UnResolverNodes.RemoveAll(predicate3);
                }
            }
            else
            {
                Predicate<Interface2> predicate4 = null;
                IFragmentBundleMetadata framgent = e.Metadata as IFragmentBundleMetadata;
                if (this.ResolvedNodes.Find(a => framgent.HostConstraint.DependentMetadata == a.Metadata) == null)
                {
                    if (predicate4 == null)
                    {
                        predicate4 = a => framgent.HostConstraint.DependentMetadata == a.Metadata;
                    }
                    Interface2 interface2 = this.UnResolverNodes.Find(predicate4);
                }
            }
        }

        private void TryResolveBundles(List<Interface2> bundles)
        {
            bundles.ForEach(new Action<Interface2>(this.MarkAsResolvable));
            bundles.ForEach(delegate (Interface2 bundle) {
                if (!bundle.IsResolved && bundle.IsResolvable)
                {
                    this.OnBundleResolving(new BundleMetadataEventArgs((IBundleMetadata) bundle.Metadata));
                    bundle.Resolve();
                    if (bundle.IsResolved)
                    {
                        this.ResolvedNodes.Add(bundle);
                        this.UnResolverNodes.Remove(bundle);
                        this.OnBundleResolved(new BundleMetadataEventArgs((IBundleMetadata) bundle.Metadata));
                    }
                    if (!bundle.IsResolvable && !bundle.IsResolvable)
                    {
                        this.MarkUnResolvable(bundles, new Interface2[] { bundle });
                    }
                }
            });
        }

        public void TryUnResolveBundles(List<Interface2> needUnResolvedNodes)
        {
            needUnResolvedNodes.RemoveAll(delegate (Interface2 a) {
                this.OnBundleUnresolving(new BundleMetadataEventArgs((IBundleMetadata) a.Metadata));
                if (a.Unresolve())
                {
                    this.ResolvedNodes.Remove(a);
                    this.UnResolverNodes.Add(a);
                    this.OnBundleUnresolved(new BundleMetadataEventArgs((IBundleMetadata) a.Metadata));
                    return true;
                }
                return false;
            });
        }

        public List<IAssemblyMetadataNode> ResolvedAssemblyMetadataNodes { get; private set; }

        public List<Interface2> ResolvedNodes { get; private set; }

        public IState State { get; private set; }

        public List<Interface2> UnResolverNodes { get; private set; }
    }
}

