namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;
    using Core.Bundle;
    using Dependency.Metadata;

    internal class DependencyAssemblyLoader : AbstractRuntimeLoader, IRuntimeLoader, IDependencyAssemblyLoader
    {
        public DependencyAssemblyLoader(IDependentAssemblyConstraint dependentAssemblyConstraint, BundleLoader load) : base(load)
        {
            DependentAssembly = dependentAssemblyConstraint;
        }

        private AssemblyLoader FindDepdentAssemblyLoader()
        {
            Predicate<AssemblyLoader> match = null;
            if (DependentAssembly.DependentMetadata == null)
            {
                return null;
            }
            AbstractBundle bundle = (AbstractBundle) base.Owner.Framework.Bundles.Find(delegate (IBundle bundle) {
                AbstractBundle bundle2 = bundle as AbstractBundle;
                return (bundle2 != null) && (bundle2.Metadata == DependentAssembly.DependentMetadata.Owner);
            });
            if (bundle == null)
            {
                return null;
            }
            if (match == null)
            {
                match = assemblyLoader => assemblyLoader.Import == DependentAssembly.DependentMetadata;
            }
            return bundle.BundleLoaderProxy.BundleLoader.AssemblyPathLoaders.Find(match);
        }

        public override Type LoadClass(string className)
        {
            AssemblyLoader loader = FindDepdentAssemblyLoader();
            if (loader != null)
            {
                return loader.LoadClass(className);
            }
            return null;
        }

        public override object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode)
        {
            AssemblyLoader loader = FindDepdentAssemblyLoader();
            if (loader != null)
            {
                return loader.LoadResource(resourceName, resourceLoadMode);
            }
            return null;
        }

        public IDependentAssemblyConstraint DependentAssembly { get; set; }
    }
}

