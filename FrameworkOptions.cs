namespace UIShell.OSGi
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class FrameworkOptions
    {
        internal FrameworkOptions(string[] pluginsDirectoryList)
        {
            PluginsDirectoryList = new List<string>(pluginsDirectoryList);
            BundlePersistentFileName = "persistent.xml";
        }

        public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public string BundlePersistentFileName { get; set; }

        public DefaultBundleState DefaultBundleState { get; set; }

        public string GlobalPersistentFile =>
            Path.Combine(BaseDirectory, BundlePersistentFileName);

        public List<string> PluginsDirectoryList { get; private set; }

        public string PluginsDirectoryName
        {
            get
            {
                if (PluginsDirectoryList.Count > 0)
                {
                    return PluginsDirectoryList[0];
                }
                return "Plugins";
            }
        }
    }
}

