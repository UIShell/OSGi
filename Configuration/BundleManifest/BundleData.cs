namespace UIShell.OSGi.Configuration.BundleManifest
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using OSGi;

    [DebuggerDisplay("Version:{Version},Path:{Path}", Name="BundleData:{SymbolicName}")]
    public class BundleData
    {
        public BundleData()
        {
            Services = new List<ServiceData>();
            Runtime = new RuntimeData();
            ExtensionPoints = new List<ExtensionPointData>();
            Extensions = new List<ExtensionData>();
        }

        public void AddExtension(ExtensionData newItem)
        {
            Extensions.Add(newItem);
        }

        public void AddExtensionPoint(ExtensionPointData newItem)
        {
            ExtensionPoints.Add(newItem);
        }

        public void AddService(ServiceData newItem)
        {
            Services.Add(newItem);
        }

        public ActivatorData Activator { get; set; }

        public BundleInfoData BundleInfo { get; set; }

        public List<ExtensionPointData> ExtensionPoints { get; private set; }

        public List<ExtensionData> Extensions { get; private set; }

        public string HostBundleSymbolicName { get; set; }

        [TypeConverter(typeof(VersionConverter))]
        public VersionRange HostBundleVersion { get; set; }

        public BundleInitializedState? InitializedState { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public RuntimeData Runtime { get; set; }

        public List<ServiceData> Services { get; set; }

        public int? StartLevel { get; set; }

        public string SymbolicName { get; set; }

        [TypeConverter(typeof(VersionConverter))]
        public System.Version Version { get; set; }

        public string Xmlns { get; set; }
    }
}

