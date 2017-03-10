namespace UIShell.OSGi.Core
{
    using System;
    using OSGi;
    using Bundle;
    using Command;
    using Service;
    using Event;

    public interface IFramework
    {
        void AddSystemService(object serviceInstance, params Type[] serviceTypes);
        void AddSystemService(Type serviceType, params object[] serviceInstances);
        IBundle GetBundle(string location);
        IBundle GetBundleByID(int bundleID);
        IBundle GetBundleBySymbolicName(string name);
        void RemoveSystemService(Type serviceType, object serviceInstance);
        bool RunCommand(string cmd);
        void Start();
        void Stop();

        BundleRepository Bundles { get; }

        GInterface0 Commands { get; }

        EventManager EventManager { get; }

        bool IsActive { get; }

        FrameworkOptions Options { get; }

        IServiceManager ServiceContainer { get; }
    }
}

