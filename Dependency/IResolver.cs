namespace UIShell.OSGi.Dependency
{
    using System;
    using System.Collections.Generic;

    internal interface IResolver
    {
        event EventHandler<BundleMetadataEventArgs> BundleResolved;

        event EventHandler<BundleMetadataEventArgs> BundleResolving;

        event EventHandler<BundleMetadataEventArgs> BundleUnresolved;

        event EventHandler<BundleMetadataEventArgs> BundleUnresolving;

        void Resolve(List<IBundleMetadata> reResolve);

        List<IAssemblyMetadataNode> ResolvedAssemblyMetadataNodes { get; }

        List<Interface2> ResolvedNodes { get; }

        IState State { get; }

        List<Interface2> UnResolverNodes { get; }
    }
}

