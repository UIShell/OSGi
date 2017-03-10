namespace UIShell.OSGi
{
    using System;

    public interface IBundle
    {
        int GetBunldeStartLevel();
        Type LoadClass(string className);
        object LoadResource(string resourceName, ResourceLoadMode loadMode);
        void Start(BundleStartOptions option);
        void Stop(BundleStopOptions option);
        void Uninstall();

        long BundleID { get; }

        BundleType BundleType { get; }

        IBundleContext Context { get; }

        string Location { get; }

        string Name { get; }

        int StartLevel { get; }

        BundleState State { get; }

        string SymbolicName { get; }

        Version Version { get; }
    }
}

