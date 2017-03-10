namespace UIShell.OSGi.Loader
{
    using System.Collections.Generic;
    using OSGi;
    using Core;

    internal interface IBundleLoader : IRuntimeLoader
    {
        List<AssemblyLoader> AssemblyPathLoaders { get; }

        IBundle Bundle { get; }

        List<DependencyAssemblyLoader> DependencyAssemblyLoaders { get; }

        List<DependencyBundleLoader> DependencyBundleLoaders { get; }

        IFramework Framework { get; }
    }
}

