namespace UIShell.OSGi.Core.Service
{
    using System.Collections.Generic;
    using System.IO;
    using OSGi;

    public interface IBundleManager
    {
        List<BundleInfo> GetAllBundles();
        IBundle InstallBundle(string location);
        IBundle InstallBundle(string location, Stream stream);
        void Start(string bundleSymbolicName);
        void Start(string bundleSymbolicName, BundleStartOptions option);
        void Stop(string bundleSymbolicName);
        void Stop(string bundleSymbolicName, BundleStopOptions option);
        void Uninstall(string bundleSymbolicName);
        void Update(string bundleSymbolicName);
    }
}

