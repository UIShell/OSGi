namespace UIShell.OSGi.Configuration.BundleManifest
{
    using OSGi;

    public class DependencyData
    {
        public string AssemblyName { get; set; }

        public VersionRange AssemblyVersion { get; set; }

        public string BundleSymbolicName { get; set; }

        public VersionRange BundleVersion { get; set; }

        public ResolutionType Resolution { get; set; }
    }
}

