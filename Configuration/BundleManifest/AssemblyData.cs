namespace UIShell.OSGi.Configuration.BundleManifest
{
    public class AssemblyData
    {
        public AssemblyData()
        {
            MultipleVersions = true;
        }

        public bool MultipleVersions { get; set; }

        public string Path { get; set; }

        public string[] PathArray
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return null;
                }
                return Path.Split(new char[] { ';' });
            }
        }

        public bool Share { get; set; }
    }
}

