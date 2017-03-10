namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UIShell.OSGi.Dependency.Metadata;

    [DebuggerDisplay("IsResolved:{IsResolved}", Name="{SymbolicName}")]
    internal class BundleMetadata : Metadata, IMetadata, IBundleMetadata
    {
        public BundleMetadata()
        {
            this.PrivateAssemblies = new List<IAssemblyMetadata>();
            this.SharedAssemblies = new List<IAssemblyMetadata>();
            this.DependentAssemblies = new List<IDependentAssemblyConstraint>();
            this.DependentBundles = new List<IDependentBundleConstraint>();
            this.AssembliesFailedToLoad = new List<IAssemblyMetadata>();
        }

        public Tuple<IDependentAssemblyConstraint, IAssemblyMetadata>[] GetResolvedDependentAssemblies() => 
            this.ResolvedDependentAssemblies;

        public Tuple<IDependentBundleConstraint, Interface1>[] GetResolvedDependentBundles() => 
            this.ResolvedDependentBundles;

        public List<IAssemblyMetadata> AssembliesFailedToLoad { get; private set; }

        public long BundleID { get; set; }

        public List<IDependentAssemblyConstraint> DependentAssemblies { get; private set; }

        public List<IDependentBundleConstraint> DependentBundles { get; private set; }

        public bool IsResolved { get; set; }

        public string Location { get; set; }

        public List<IAssemblyMetadata> PrivateAssemblies { get; private set; }

        internal Tuple<IDependentAssemblyConstraint, IAssemblyMetadata>[] ResolvedDependentAssemblies { get; set; }

        internal Tuple<IDependentBundleConstraint, Interface1>[] ResolvedDependentBundles { get; set; }

        public List<IAssemblyMetadata> SharedAssemblies { get; private set; }

        public string SymbolicName { get; set; }
    }
}

