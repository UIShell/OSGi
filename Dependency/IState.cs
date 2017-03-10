namespace UIShell.OSGi.Dependency
{
    using System;
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IState
    {
        event EventHandler<BundleMetadataEventArgs> BundleAdded;

        event EventHandler<BundleMetadataEventArgs> BundleRemovalPending;

        event EventHandler<BundleMetadataEventArgs> BundleRemoved;

        void AddBundle(IBundleMetadata bundle);
        void AddResolverError(IResolverError error);
        IBundleMetadata GetBundleByID(long bundleID);
        IBundleMetadata GetBundleByLocation(string location);
        List<IBundleMetadata> GetBundleBySymbolicName(string symbolicName);
        void RemoveBundle(IBundleMetadata bundleMetadata);
        void RemoveResolverError(IResolverError error);
        void Resolve();
        void ResolveBundle(IBundleMetadata bundle, bool isResolved, Tuple<IHostConstraint, Interface1>[] resolvedHosts, Tuple<IDependentBundleConstraint, Interface1>[] resolvedDependentBundles, Tuple<IDependentAssemblyConstraint, IAssemblyMetadata> resolvedDependentAssemblies);

        List<IBundleMetadata> Bundles { get; }

        IStateDelta Changes { get; }

        bool IsEmpty { get; }

        bool IsResolved { get; }

        IMetadataBuilder MetadataBuilder { get; set; }

        List<IBundleMetadata> ResolvedBundles { get; }

        IResolver Resolver { get; }

        List<IAssemblyMetadata> SharedAssemblies { get; }

        long TimeStamp { get; }

        List<IBundleMetadata> UnResolvedBundles { get; }
    }
}

