namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;
    using Core;
    using Core.Bundle;
    using Dependency.Metadata;

    internal class BundleLoaderProxyFactory
    {
        internal static IBundleLoader CreateBundleLoader(IBundle bundle, Framework framework)
        {
            var metadataBuilder = framework.FrameworkAdaptor.State.MetadataBuilder;
            var bundleMetadata = framework.GetBundleMetadata(bundle.BundleID);
            var bundle2 = bundle as AbstractBundle;
            if (bundle2 == null)
            {
                throw new NotSupportedException();
            }

            var load = bundle2.CreateBundleLoader();
            bundleMetadata.SharedAssemblies.ForEach(delegate (IAssemblyMetadata a) {
                load.AssemblyPathLoaders.Add(new AssemblyLoader(a, load));
            });
            bundleMetadata.PrivateAssemblies.ForEach(delegate (IAssemblyMetadata a) {
                load.AssemblyPathLoaders.Add(new AssemblyLoader(a, load));
            });
            bundleMetadata.DependentAssemblies.ForEach(delegate (IDependentAssemblyConstraint a) {
                load.DependencyAssemblyLoaders.Add(new DependencyAssemblyLoader(a, load));
            });
            bundleMetadata.DependentBundles.ForEach(delegate (IDependentBundleConstraint a) {
                load.DependencyBundleLoaders.Add(new DependencyBundleLoader(a, load));
            });
            return load;
        }
    }
}

