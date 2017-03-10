namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using OSGi;
    using Service;
    using Persistent;
    using Utility;

    [Serializable]
    public class BundlePersistent : IBundlePersistent
    {
        public BundlePersistent()
        {
            InstalledBundleLocation = new List<string>();
            UnInstalledBundleLocation = new List<UnInstallBundleOption>();
        }

        private bool IsSubDirectoryOrEqualPluginsDirectory(string path, List<string> pluginsDirectoryList)
        {
            var fullPath = PathUtility.GetFullPath(path);
            var str2 = string.Empty;
            foreach (var str3 in pluginsDirectoryList)
            {
                str2 = PathUtility.GetFullPath(str3);
                if (fullPath.StartsWith(str2))
                {
                    return true;
                }
            }
            return false;
        }

        public object Load(string file)
        {
            var persistent = PersistentHelper.Load(file, base.GetType()) as BundlePersistent;
            if (persistent != null)
            {
                InstalledBundleLocation = persistent.InstalledBundleLocation;
                UnInstalledBundleLocation = persistent.UnInstalledBundleLocation;
            }
            return persistent;
        }

        public void Save(string file)
        {
            PersistentHelper.Save(file, this);
        }

        public void SaveInstallLocation(string bundleLocation)
        {
            if (!InstalledBundleLocation.Contains(bundleLocation) && !IsSubDirectoryOrEqualPluginsDirectory(bundleLocation, BundleRuntime.Instance.Framework.Options.PluginsDirectoryList))
            {
                InstalledBundleLocation.Add(bundleLocation);
            }
            UnInstalledBundleLocation.RemoveAll(item => item.Location == bundleLocation);
        }

        public void SaveUnInstallLocation(string path)
        {
            SaveUnInstallLocation(path, true);
        }

        public void SaveUnInstallLocation(string path, bool needRemove)
        {
            if (!UnInstalledBundleLocation.Exists(item => item.Location == path))
            {
                var option = new UnInstallBundleOption {
                    Location = path,
                    NeedRemove = needRemove
                };
                UnInstalledBundleLocation.Add(option);
            }
            InstalledBundleLocation.Remove(path);
        }

        public List<string> InstalledBundleLocation { get; set; }

        public List<UnInstallBundleOption> UnInstalledBundleLocation { get; set; }
    }
}

