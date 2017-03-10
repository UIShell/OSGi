namespace UIShell.OSGi.Dependency.Resolver
{
    using System.Collections.Generic;

    internal interface Interface2 : IResolvable, IMetadataNode
    {
        void Initialize();

        long BundleID { get; }

        List<IDependentAssemblyConstraintNode> DependentAssemblyNodes { get; }

        List<IDependentBundleConstraintNode> DependentBundleNodes { get; }

        List<IAssemblyMetadataNode> PrivateAssemblyNodes { get; }

        List<IAssemblyMetadataNode> SharedAssemblyNodes { get; }

        string SymbolicName { get; }
    }
}

