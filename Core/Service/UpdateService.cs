namespace UIShell.OSGi.Core.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OSGi;
    using Configuration.BundleManifest;
    using Utility;

    internal class UpdateService
    {
        private static Tuple<Version, string> FindUpgradeVersion(string appSymbolicNameFolder, Version currentVersion)
        {
            Version version = null;
            if (Directory.Exists(appSymbolicNameFolder))
            {
                string[] directories = Directory.GetDirectories(appSymbolicNameFolder);
                string str = string.Empty;
                string[] strArray2 = directories;
                int index = 0;
                while (true)
                {
                    if (index >= strArray2.Length)
                    {
                        break;
                    }
                    string path = strArray2[index];
                    try
                    {
                        Version newVersion = new Version(new DirectoryInfo(path).Name);
                        if (VersionUtility.Compatible(currentVersion, newVersion) && ((version == null) || (newVersion > version)))
                        {
                            version = newVersion;
                            str = path;
                        }
                    }
                    catch
                    {
                    }
                    index++;
                }
                if (version != null)
                {
                    return new Tuple<Version, string>(version, str);
                }
            }
            return null;
        }

        public static void Update(ICollection<BundleData> bundleDatas, string updateFolder)
        {
            if (Directory.Exists(updateFolder) && (Directory.GetDirectories(updateFolder).Length != 0))
            {
                foreach (BundleData data in bundleDatas)
                {
                    Tuple<Version, string> tuple = FindUpgradeVersion(Path.Combine(updateFolder, data.SymbolicName), data.Version);
                    if (tuple != null)
                    {
                        FileLogUtility.Inform($"Find a new version of bundle '{data.SymbolicName}', the new version is '{tuple.Item1}', while the old version is '{data.Version}'.");
                        UpdateFromFolder(tuple.Item2, data);
                    }
                }
            }
        }

        public static void Update(BundleData bundleData, string updateFolder)
        {
            Update(new List<BundleData>(new BundleData[] { bundleData }), updateFolder);
        }

        private static void UpdateFromFolder(string upgradPackae, BundleData item)
        {
            FileUtility.CopyFilesDirs(upgradPackae, item.Path, true);
            string symbolicName = item.SymbolicName;
            Version version = item.Version;
            IBundleInstallerService firstOrDefaultService = BundleRuntime.Instance.GetFirstOrDefaultService<IBundleInstallerService>();
            firstOrDefaultService.BundleDatas.Remove(item.SymbolicName);
            BundleData data = firstOrDefaultService.CreateBundleData(item.Path);
            string text2 = data.SymbolicName;
            Version version2 = data.Version;
            FileLogUtility.Inform($"Upgrade the bundle '{item.SymbolicName}' from version '{version}' to '{version2}'.");
        }
    }
}

