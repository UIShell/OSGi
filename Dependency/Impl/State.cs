namespace UIShell.OSGi.Dependency.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;
    using UIShell.OSGi.Utility;

    internal class State : IState
    {
        public event EventHandler<BundleMetadataEventArgs> BundleAdded;

        public event EventHandler<BundleMetadataEventArgs> BundleRemovalPending;

        public event EventHandler<BundleMetadataEventArgs> BundleRemoved;

        public State()
        {
            this.SharedAssemblies = new List<IAssemblyMetadata>();
            this.Bundles = new List<IBundleMetadata>();
            this.ResolvedBundles = new List<IBundleMetadata>();
            this.UnResolvedBundles = new List<IBundleMetadata>();
            this.Resolver = new UIShell.OSGi.Dependency.Impl.Resolver(this);
            this.Changes = new StateDelta();
        }

        public void AddBundle(IBundleMetadata bundle)
        {
            this.Bundles.Add(bundle);
            this.OnAddedBundle(bundle);
        }

        public void AddResolverError(IResolverError error)
        {
            throw new NotImplementedException();
        }

        public IBundleMetadata GetBundleByID(long bundleID) => 
            this.Bundles.Find(metadata => metadata.BundleID == bundleID);

        public IBundleMetadata GetBundleByLocation(string location) => 
            this.Bundles.Find(item => item.Location == location);

        public List<IBundleMetadata> GetBundleBySymbolicName(string symbolicName) => 
            this.Bundles.FindAll(a => a.SymbolicName == symbolicName);

        private void OnAddedBundle(IBundleMetadata bundleDescription)
        {
            this.Changes.RecordBundleAdded(bundleDescription);
            if (this.BundleAdded != null)
            {
                this.BundleAdded(this, new BundleMetadataEventArgs(bundleDescription));
            }
        }

        private void OnRemovalPending(IBundleMetadata metadata)
        {
            this.Changes.RecordBundleRemovalPending(metadata);
            if (this.BundleRemovalPending != null)
            {
                this.BundleRemovalPending(this, new BundleMetadataEventArgs(metadata));
            }
        }

        private void OnRemovedBundle(IBundleMetadata bundleDescription)
        {
            this.Changes.RecordBundleRemoved(bundleDescription);
            if (this.BundleRemoved != null)
            {
                this.BundleRemoved(this, new BundleMetadataEventArgs(bundleDescription));
            }
        }

        public void RemoveBundle(IBundleMetadata bundleMetadata)
        {
            if (bundleMetadata != null)
            {
                IHostBundleMetadataNode node = this.Resolver.ResolvedNodes.Find(node => node.Metadata == bundleMetadata) as IHostBundleMetadataNode;
                if ((node != null) && node.IsReferenced())
                {
                    this.Changes.RecordBundleRemovalPending(bundleMetadata);
                    this.OnRemovalPending(bundleMetadata);
                }
                else
                {
                    this.Bundles.Remove(bundleMetadata);
                    this.OnRemovedBundle(bundleMetadata);
                }
            }
        }

        public void RemoveResolverError(IResolverError error)
        {
            throw new NotImplementedException();
        }

        public void Resolve()
        {
            Predicate<IBundleMetadata> match = null;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                List<IBundleMetadata> reResolve = new List<IBundleMetadata>(this.Bundles);
                if (match == null)
                {
                    match = a => this.ResolvedBundles.Contains(a as Interface1);
                }
                reResolve.RemoveAll(match);
                this.Resolver.Resolve(reResolve);
                this.IsResolved = !reResolve.Exists(a => !a.IsResolved && (a is IHostBundleMetadataNode));
            }
            finally
            {
                stopwatch.Stop();
                FileLogUtility.Verbose(string.Format(Messages.ResolveBundlesTimeCounter, stopwatch.ElapsedMilliseconds));
            }
        }

        public void ResolveBundle(IBundleMetadata bundleMetadata, bool isResolved, Tuple<IHostConstraint, Interface1>[] resolvedHosts, Tuple<IDependentBundleConstraint, Interface1>[] resolvedDependentBundles, Tuple<IDependentAssemblyConstraint, IAssemblyMetadata> resolvedDependentAssemblies)
        {
        }

        public List<IBundleMetadata> Bundles { get; private set; }

        public IStateDelta Changes { get; private set; }

        public bool IsEmpty =>
            (this.Bundles.Count == 0);

        public bool IsResolved { get; private set; }

        public IMetadataBuilder MetadataBuilder { get; set; }

        public List<IBundleMetadata> ResolvedBundles { get; private set; }

        public IResolver Resolver { get; private set; }

        public List<IAssemblyMetadata> SharedAssemblies { get; private set; }

        public long TimeStamp { get; private set; }

        public List<IBundleMetadata> UnResolvedBundles { get; private set; }
    }
}

