namespace UIShell.OSGi.Dependency.Metadata
{
    using System;
    using System.Collections.Generic;

    internal interface IBundleMetadata : IMetadata
    {
        Tuple<IDependentAssemblyConstraint, IAssemblyMetadata>[] GetResolvedDependentAssemblies();
        Tuple<IDependentBundleConstraint, Interface1>[] GetResolvedDependentBundles();

        List<IAssemblyMetadata> AssembliesFailedToLoad { get; }

        long BundleID { get; set; }

        List<IDependentAssemblyConstraint> DependentAssemblies { get; }

        List<IDependentBundleConstraint> DependentBundles { get; }

        bool IsResolved { get; set; }

        string Location { get; set; }

        List<IAssemblyMetadata> PrivateAssemblies { get; }

        List<IAssemblyMetadata> SharedAssemblies { get; }

        string SymbolicName { get; set; }
    }
}

