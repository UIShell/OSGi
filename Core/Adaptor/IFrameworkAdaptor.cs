namespace UIShell.OSGi.Core.Adaptor
{
    using System.Collections.Generic;
    using Configuration.BundleManifest;
    using Dependency;

    internal interface IFrameworkAdaptor
    {
        void CompactStorage();
        void CreateSystemBundle();
        void Initialize();
        void InitializeStorage();

        List<BundleData> InstalledBundles { get; }

        IState State { get; set; }
    }
}

