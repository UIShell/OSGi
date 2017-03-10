namespace UIShell.OSGi.Core.Service
{
    public class UnInstallBundleOption
    {
        public UnInstallBundleOption()
        {
            NeedRemove = false;
        }

        public string Location { get; set; }

        public bool NeedRemove { get; set; }
    }
}

