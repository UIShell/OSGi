namespace UIShell.OSGi.Core.Service
{
    using OSGi;

    public interface GInterface1
    {
        void ChangeBundleStartLevel(IBundle bundle, int startLevel);
        void ChangeStartLevel(int startLevel);

        int InitialBundleStartLevel { get; set; }

        int StartLevel { get; set; }
    }
}

