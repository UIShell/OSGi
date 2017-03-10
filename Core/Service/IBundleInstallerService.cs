namespace UIShell.OSGi.Core.Service
{
    using System.Collections.Generic;
    using System.IO;
    using Configuration.BundleManifest;

    public interface IBundleInstallerService
    {
        BundleData CreateBundleData(string bundleDir);
        BundleData CreateBundleData(string bundleDir, Stream stream);
        BundleData FindBundleContainPath(string path);
        BundleData GetBundleDataByName(string symbolicName);
        string GetBundlePath(string symbolicName);
        bool InstallBundles();

        IDictionary<string, BundleData> BundleDatas { get; }

        string GlobalPersistentFile { get; }

        string UpdateFolder { get; }
    }
}

