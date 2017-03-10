namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using OSGi;
    using Configuration;
    using Configuration.BundleManifest;
    using Core;
    using Service;
    using Utility;

    internal class BundleInstallerService : IBundleInstallerService
    {
        private IDictionary<string, BundleData> _bundleDatas = new Dictionary<string, BundleData>();
        private IFramework _framework;

        public BundleInstallerService(IFramework framework, string globalPersistFile, string[] pluginsDirectoryList)
        {
            _framework = framework;
            GlobalPersistentFile = globalPersistFile;
            PluginsDirectoryList = pluginsDirectoryList;
        }

        public BundleData CreateBundleData(string bundleDir)
        {
            bundleDir = PathUtility.GetFullPath(bundleDir);
            string path = Path.Combine(bundleDir, "Manifest.xml");
            if (!File.Exists(path))
            {
                return null;
            }
            BundleData data = null;
            try
            {
                data = ManifestParser.CreateBundleData(path);
            }
            catch (Exception exception)
            {
                FileLogUtility.Error(string.Format(Messages.InstallBundleFailed, bundleDir, exception.Message));
            }
            if (data != null)
            {
                if (string.IsNullOrEmpty(data.SymbolicName))
                {
                    _framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Error, new Exception(string.Format(Messages.SymbolicNameIsEmpty, bundleDir))));
                    return null;
                }
                if (_bundleDatas.ContainsKey(data.SymbolicName))
                {
                    _framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Error, new Exception(string.Format(Messages.SymbolicNameDuplicated, bundleDir, _bundleDatas[data.SymbolicName].Path))));
                    return null;
                }
                data.Path = bundleDir;
                _bundleDatas.Add(data.SymbolicName, data);
            }
            return data;
        }

        public BundleData CreateBundleData(string bundleDir, Stream stream)
        {
            if (stream == null)
            {
                return CreateBundleData(bundleDir);
            }
            BundleData data = null;
            try
            {
                ManifestParser.CreateBundleData(Path.Combine(bundleDir, "Manifest.xml"), stream);
            }
            catch (Exception exception)
            {
                FileLogUtility.Error(string.Format(Messages.InstallBundleFailed, bundleDir, exception.Message));
            }
            if (data != null)
            {
                data.Path = bundleDir;
                _bundleDatas.Add(data.SymbolicName, data);
            }
            return data;
        }

        public BundleData FindBundleContainPath(string path)
        {
            BundleData data2;
            using (IEnumerator<BundleData> enumerator = BundleDatas.Values.GetEnumerator())
            {
                BundleData current;
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if (path.StartsWith(current.Path, StringComparison.CurrentCultureIgnoreCase))
                    {
                        goto Label_0034;
                    }
                }
                return null;
            Label_0034:
                data2 = current;
            }
            return data2;
        }

        internal BundleData GetBundleData(string location)
        {
            BundleData data;
            using (IEnumerator<KeyValuePair<string, BundleData>> enumerator = _bundleDatas.GetEnumerator())
            {
                KeyValuePair<string, BundleData> current;
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if (current.Value.Path.CompareTo(location) == 0)
                    {
                        goto Label_0034;
                    }
                }
                return null;
            Label_0034:
                data = current.Value;
            }
            return data;
        }

        public BundleData GetBundleDataByName(string symbolicName)
        {
            BundleData data;
            using (IEnumerator<KeyValuePair<string, BundleData>> enumerator = _bundleDatas.GetEnumerator())
            {
                KeyValuePair<string, BundleData> current;
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if (current.Value.SymbolicName.CompareTo(symbolicName) == 0)
                    {
                        goto Label_0034;
                    }
                }
                return null;
            Label_0034:
                data = current.Value;
            }
            return data;
        }

        public string GetBundlePath(string symbolicName)
        {
            BundleData data;
            if (!string.IsNullOrEmpty(symbolicName) && _bundleDatas.TryGetValue(symbolicName, out data))
            {
                return data.Path;
            }
            return string.Empty;
        }

        public bool InstallBundles()
        {
            if (File.Exists(GlobalPersistentFile))
            {
                Persistenter.Load(GlobalPersistentFile);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<string> pluginDirectoryList = new List<string>();
            string path = string.Empty;
            foreach (string str in PluginsDirectoryList)
            {
                path = str;
                if (!Directory.Exists(path))
                {
                    path = PathUtility.GetFullPath(path);
                }
                if (Directory.Exists(path))
                {
                    foreach (string str3 in Directory.GetFiles(path, "Manifest.xml", SearchOption.AllDirectories))
                    {
                        pluginDirectoryList.Add(Directory.GetParent(Path.GetFullPath(str3)).FullName);
                    }
                }
                else
                {
                    FileLogUtility.Warn(string.Format(Messages.PluginsDirectoryNotExist, path, str));
                }
            }
            stopwatch.Stop();
            FileLogUtility.Verbose(string.Format(Messages.TimeSpentForBundlesFound, stopwatch.ElapsedMilliseconds, pluginDirectoryList.Count));
            stopwatch.Start();
            InstallBundles(pluginDirectoryList);
            stopwatch.Stop();
            FileLogUtility.Verbose(string.Format(Messages.TimeSpentForBundleManifestParsed, stopwatch.ElapsedMilliseconds, pluginDirectoryList.Count));
            return true;
        }

        private void InstallBundles(List<string> pluginDirectoryList)
        {
            Reset();
            RemoveBundlesToBeDeleted(pluginDirectoryList);
            pluginDirectoryList.ForEach((Action<string>) (pluginDirectory => CreateBundleData(pluginDirectory)));
            InstallBundlesToBeInstalled(pluginDirectoryList);
        }

        private void InstallBundlesToBeInstalled(List<string> pluginDirectoryList)
        {
            Action<string> action = null;
            if (Persistenter != null)
            {
                if (action == null)
                {
                    action = delegate (string bundleLocation) {
                        string item = PathUtility.GetFullPath(bundleLocation);
                        if (!pluginDirectoryList.Contains(item))
                        {
                            CreateBundleData(bundleLocation);
                        }
                    };
                }
                Persistenter.InstalledBundleLocation.ForEach(action);
            }
        }

        private static bool IsSamePath(string path1, string path2) => 
            (string.Compare(PathUtility.GetFullPath(path1).ToUpperInvariant().TrimEnd(new char[] { '\\' }), PathUtility.GetFullPath(path2).ToUpperInvariant().TrimEnd(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase) == 0);

        private void RemoveBundlesToBeDeleted(List<string> pluginDirectoryList)
        {
            Predicate<string> match = null;
            if (Persistenter != null)
            {
                if (match == null)
                {
                    match = delegate (string pluginDirectory) {
                        UnInstallBundleOption item = Persistenter.UnInstalledBundleLocation.Find(a => IsSamePath(a.Location, pluginDirectory));
                        if (item == null)
                        {
                            return false;
                        }
                        if (item.NeedRemove)
                        {
                            try
                            {
                                Directory.Delete(item.Location, true);
                            }
                            catch (Exception exception)
                            {
                                FileLogUtility.Error(string.Format(Messages.DeleteDirectoryFailed, item.Location, exception.Message));
                            }
                            try
                            {
                                Persistenter.UnInstalledBundleLocation.Remove(item);
                                Persistenter.Save(GlobalPersistentFile);
                            }
                            catch (Exception exception2)
                            {
                                FileLogUtility.Error(string.Format(Messages.DeleteDirectoryFailed, item.Location, exception2.Message));
                            }
                        }
                        return true;
                    };
                }
                pluginDirectoryList.RemoveAll(match);
            }
        }

        private void Reset()
        {
            _bundleDatas.Clear();
        }

        public string BaseDirectory =>
            AppDomain.CurrentDomain.BaseDirectory;

        public IDictionary<string, BundleData> BundleDatas =>
            _bundleDatas;

        public string GlobalPersistentFile { get; private set; }

        public IBundlePersistent Persistenter =>
            _framework.ServiceContainer.GetFirstOrDefaultService<IBundlePersistent>();

        public string[] PluginsDirectoryList { get; private set; }

        public string UpdateFolder =>
            Path.Combine(BaseDirectory, "Update");
    }
}

