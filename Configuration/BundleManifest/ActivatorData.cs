namespace UIShell.OSGi.Configuration.BundleManifest
{
    public class ActivatorData
    {
        public ActivatorData()
        {
            Policy = ActivatorPolicy.Immediate;
        }

        public ActivatorPolicy Policy { get; set; }

        public string Type { get; set; }
    }
}

