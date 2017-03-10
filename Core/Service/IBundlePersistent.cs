namespace UIShell.OSGi.Core.Service
{
    using System.Collections.Generic;

    public interface IBundlePersistent
    {
        object Load(string file);
        void Save(string file);
        void SaveInstallLocation(string path);
        void SaveUnInstallLocation(string path);
        void SaveUnInstallLocation(string path, bool needRemove);

        List<string> InstalledBundleLocation { get; set; }

        List<UnInstallBundleOption> UnInstalledBundleLocation { get; set; }
    }
}

