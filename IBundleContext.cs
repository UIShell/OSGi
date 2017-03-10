namespace UIShell.OSGi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Core;

    public interface IBundleContext : IDisposable
    {
        event EventHandler<BundleLazyActivatedEventArgs> BundleLazyActivated;

        event EventHandler<BundleStateChangedEventArgs> BundleStateChanged;

        event EventHandler<ExtensionEventArgs> ExtensionChanged;

        event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged;

        event EventHandler<FrameworkEventArgs> FrameworkStateChanged;

        event EventHandler<ServiceEventArgs> ServiceChanged;

        event EventHandler<BundleStateChangedEventArgs> SyncBundleStateChanged;

        void AddExtension(string point, Extension extension);
        void AddService<T>(T serviceInstance);
        void AddService(Type serviceType, object serviceInstance);
        IBundle GetBundleByID(long bundleID);
        IBundle GetBundleByLocation(string location);
        IBundle GetBundleBySymbolicName(string symbolicName);
        List<IBundle> GetBundles();
        ExtensionPoint GetExtensionPoint(string point);
        List<ExtensionPoint> GetExtensionPoints();
        List<Extension> GetExtensions(string extensionPoint);
        T GetFirstOrDefaultService<T>();
        object GetFirstOrDefaultService(string serviceTypFullName);
        object GetFirstOrDefaultService(Type serviceType);
        List<T> GetService<T>();
        List<object> GetService(string serviceTypeName);
        List<object> GetService(Type serviceType);
        IBundle InstallBundle(string location);
        IBundle InstallBundle(string location, Stream stream);
        void RemoveExtension(Extension extension);
        void RemoveService<T>(object serviceInstance);
        void RemoveService(Type serviceType, object serviceInstance);

        IBundle Bundle { get; }

        IFramework Framework { get; }

        bool IsDisposed { get; }
    }
}

