namespace UIShell.OSGi.Core.Bundle
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OSGi;
    using Core;
    using Service;
    using Utility;

    internal class BundleContext : IDisposable, IBundleContext
    {
        private volatile bool _IsDisposed;

        public event EventHandler<BundleLazyActivatedEventArgs> BundleLazyActivated;

        public event EventHandler<BundleStateChangedEventArgs> BundleStateChanged;

        public event EventHandler<ExtensionEventArgs> ExtensionChanged;

        public event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged;

        public event EventHandler<FrameworkEventArgs> FrameworkStateChanged;

        public event EventHandler<ServiceEventArgs> ServiceChanged;

        public event EventHandler<BundleStateChangedEventArgs> SyncBundleStateChanged;

        public BundleContext(AbstractBundle bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "BundleContext.bundle");
            Bundle = bundle;
            Framework = bundle.Framework;
            AssertUtility.ArgumentNotNull(Framework, "Framework");
            ExtensionManager = GetFirstOrDefaultService<IExtensionManager>();
            AssertUtility.ArgumentNotNull(ExtensionManager, "ExtensionManager");
            ExtensionManager.ExtensionChanged += new EventHandler<ExtensionEventArgs>(OnExtensionChanged);
            ExtensionManager.ExtensionPointChanged += new EventHandler<ExtensionPointEventArgs>(OnExtensionPointChanged);
            Framework.EventManager.AddFrameworkEventListener(new EventHandler<FrameworkEventArgs>(OnFrameworkStateChanged));
            Framework.EventManager.AddBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(OnBundleStateChanged), false);
            Framework.EventManager.AddBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(OnSyncBundleStateChanged), true);
            Framework.EventManager.AddServiceEventListener(new EventHandler<ServiceEventArgs>(OnServiceChanged));
            Framework.EventManager.AddBundleLazyActivatedEventListener(new EventHandler<BundleLazyActivatedEventArgs>(OnBundleLazyActivated));
        }

        public void AddExtension(string point, Extension extension)
        {
            if (!IsDisposed)
            {
                GetFirstOrDefaultService<IExtensionManager>().AddExtension(point, extension);
            }
        }

        public void AddService<T>(T serviceInstance)
        {
            if (!IsDisposed)
            {
                AddService(typeof(T), serviceInstance);
            }
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            if (!IsDisposed)
            {
                Framework.ServiceContainer.AddService(Bundle, serviceType, new object[] { serviceInstance });
            }
        }

        public void CheckValid()
        {
            if (IsDisposed)
            {
                throw new BundleException(string.Format(Messages.BundleContextDisposed, Bundle.SymbolicName, Bundle.Version));
            }
        }

        public IBundle GetBundleByID(long bundleID)
        {
            CheckValid();
            return Framework.Bundles.GetBundle(bundleID);
        }

        public IBundle GetBundleByLocation(string location)
        {
            CheckValid();
            return Framework.Bundles.GetBundle(location);
        }

        public IBundle GetBundleBySymbolicName(string name)
        {
            CheckValid();
            return Framework.GetBundleBySymbolicName(name);
        }

        public List<IBundle> GetBundles()
        {
            CheckValid();
            return Framework.Bundles;
        }

        public ExtensionPoint GetExtensionPoint(string point)
        {
            if (!IsDisposed)
            {
                return GetFirstOrDefaultService<IExtensionManager>().GetExtensionPoint(point);
            }
            return null;
        }

        public List<ExtensionPoint> GetExtensionPoints()
        {
            if (!IsDisposed)
            {
                return GetFirstOrDefaultService<IExtensionManager>().GetExtensionPoints(Bundle);
            }
            return null;
        }

        public List<Extension> GetExtensions(string extensionPoint)
        {
            if (!IsDisposed)
            {
                return GetFirstOrDefaultService<IExtensionManager>().GetExtensions(extensionPoint);
            }
            return new List<Extension>();
        }

        public T GetFirstOrDefaultService<T>()
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetFirstOrDefaultService<T>();
            }
            return default(T);
        }

        private object GetFirstOrDefaultService(string serviceType)
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetFirstOrDefaultService(serviceType);
            }
            return null;
        }

        public object GetFirstOrDefaultService(Type serviceType)
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetFirstOrDefaultService(serviceType);
            }
            return null;
        }

        public List<T> GetService<T>()
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetService<T>();
            }
            return new List<T>();
        }

        private List<object> GetService(string serviceType)
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetService(serviceType);
            }
            return new List<object>();
        }

        public List<object> GetService(Type serviceType)
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetService(serviceType);
            }
            return new List<object>();
        }

        public IBundle InstallBundle(string location)
        {
            CheckValid();
            return Framework.ServiceContainer.GetFirstOrDefaultService<IBundleManager>().InstallBundle(location);
        }

        public IBundle InstallBundle(string location, Stream stream)
        {
            CheckValid();
            return Framework.ServiceContainer.GetFirstOrDefaultService<IBundleManager>().InstallBundle(location, stream);
        }

        private void OnBundleLazyActivated(object sender, BundleLazyActivatedEventArgs e)
        {
            if (BundleLazyActivated != null)
            {
                BundleLazyActivated(sender, e);
            }
        }

        private void OnBundleStateChanged(object sender, BundleStateChangedEventArgs e)
        {
            if (BundleStateChanged != null)
            {
                BundleStateChanged(sender, e);
            }
        }

        private void OnExtensionChanged(object sender, ExtensionEventArgs e)
        {
            if (ExtensionChanged != null)
            {
                ExtensionChanged(sender, e);
            }
        }

        private void OnExtensionPointChanged(object sender, ExtensionPointEventArgs e)
        {
            if (ExtensionPointChanged != null)
            {
                ExtensionPointChanged(sender, e);
            }
        }

        private void OnFrameworkStateChanged(object sender, FrameworkEventArgs e)
        {
            if (FrameworkStateChanged != null)
            {
                FrameworkStateChanged(sender, e);
            }
        }

        private void OnServiceChanged(object sender, ServiceEventArgs e)
        {
            if (ServiceChanged != null)
            {
                ServiceChanged(sender, e);
            }
        }

        private void OnSyncBundleStateChanged(object sender, BundleStateChangedEventArgs e)
        {
            if (SyncBundleStateChanged != null)
            {
                SyncBundleStateChanged(sender, e);
            }
        }

        public void RemoveExtension(Extension extension)
        {
            if (!IsDisposed)
            {
                GetFirstOrDefaultService<IExtensionManager>().RemoveExtension(extension);
            }
        }

        public void RemoveService(object serviceInstance)
        {
            if (!IsDisposed)
            {
                Framework.ServiceContainer.RemoveService(Bundle, serviceInstance);
            }
        }

        public void RemoveService<T>(object serviceInstance)
        {
            if (!IsDisposed)
            {
                Framework.ServiceContainer.RemoveService<T>(Bundle, serviceInstance);
            }
        }

        public void RemoveService(Type serviceType, object serviceInstance)
        {
            if (!IsDisposed)
            {
                Framework.ServiceContainer.RemoveService(Bundle, serviceType, serviceInstance);
            }
        }

        private void Stop()
        {
            CheckValid();
            ExtensionManager.ExtensionChanged -= new EventHandler<ExtensionEventArgs>(OnExtensionChanged);
            ExtensionManager.ExtensionPointChanged -= new EventHandler<ExtensionPointEventArgs>(OnExtensionPointChanged);
            Framework.EventManager.RemoveFrameworkEventListener(new EventHandler<FrameworkEventArgs>(OnFrameworkStateChanged));
            Framework.EventManager.RemoveBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(OnBundleStateChanged), false);
            Framework.EventManager.RemoveBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(OnSyncBundleStateChanged), true);
            Framework.EventManager.RemoveServiceEventListener(new EventHandler<ServiceEventArgs>(OnServiceChanged));
            Framework.EventManager.RemovedBundleLazyActivatedEventListener(new EventHandler<BundleLazyActivatedEventArgs>(OnBundleLazyActivated));
        }

        void IDisposable.Dispose()
        {
            Stop();
            IsDisposed = true;
            Framework = null;
        }

        object IBundleContext.GetFirstOrDefaultService(string serviceTypFullName)
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetFirstOrDefaultService(serviceTypFullName);
            }
            return null;
        }

        List<object> IBundleContext.GetService(string serviceTypeName)
        {
            if (!IsDisposed)
            {
                return Framework.ServiceContainer.GetService(serviceTypeName);
            }
            return new List<object>();
        }

        public IBundle Bundle { get; private set; }

        private IExtensionManager ExtensionManager { get; set; }

        public IFramework Framework { get; internal set; }

        public bool IsDisposed
        {
            get
            {
                return _IsDisposed;
            }
            internal set
            {
                _IsDisposed = value;
            }
        }
    }
}

