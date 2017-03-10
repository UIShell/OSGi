namespace UIShell.OSGi.Core
{
    using System;
    using System.Collections.Generic;
    using Adaptor;
    using Bundle;
    using Command;
    using Configuration.BundleManifest;
    using Console;
    using Dependency.Metadata;
    using Event;
    using OSGi;
    using Service;
    using Service.Impl;
    using Utility;

    internal class Framework : IDisposable, IFramework
    {
        private object _lockObject = new object();
        private List<ServiceItem> _pendingSystemServices = new List<ServiceItem>();

        public Framework()
        {
            this.EventManager = new EventManager();
            this.ServiceContainer = new ServiceManager(this);
            this.Bundles = new BundleRepository(this);
            this.FrameworkAdaptor = new FrameworkAdaptor(this);
            this.StartLevelManager = new StartLevelManager(this);
            this.Commands = new CommandRepository(this);
        }

        internal Tuple<IBundle, IBundleMetadata> AddBundleData(BundleData bundleData)
        {
            this.FrameworkAdaptor.InstalledBundles.Add(bundleData);
            IBundle item = this.ServiceContainer.GetFirstOrDefaultService<IBundleFactory>().CreateBundle(bundleData);
            this.Bundles.Add(item);
            IBundleMetadata bundle = this.FrameworkAdaptor.State.MetadataBuilder.BuildBundleMetadata(bundleData, item.BundleID);
            this.FrameworkAdaptor.State.AddBundle(bundle);
            return TupleUtility.CreateTuple<IBundle, IBundleMetadata>(item, bundle);
        }

        public void AddCommand(ICommandAdaptor cmd)
        {
            AssertUtility.ArgumentNotNull(cmd, "cmd");
            this.Commands.Commands.Add(cmd);
        }

        public void AddCommand(string cmd, ICommandAdaptor cmdAdaptor)
        {
            this.Commands.NamedCommands.Add(cmd, cmdAdaptor);
        }

        public void AddSystemService(object serviceInstance, params Type[] serviceTypes)
        {
            if ((serviceTypes == null) || (serviceInstance == null))
            {
                throw new ArgumentNullException();
            }
            if (this.SystemBundle == null)
            {
                if (serviceTypes != null)
                {
                    foreach (Type type in serviceTypes)
                    {
                        this._pendingSystemServices.Add(new ServiceItem(type, serviceInstance));
                    }
                }
            }
            else
            {
                this.ServiceContainer.AddService(this.SystemBundle, serviceInstance, serviceTypes);
            }
        }

        public void AddSystemService(Type serviceType, params object[] serviceInstances)
        {
            if ((serviceType == null) || (serviceInstances == null))
            {
                throw new ArgumentNullException();
            }
            if (this.SystemBundle == null)
            {
                if (serviceInstances != null)
                {
                    foreach (object obj2 in serviceInstances)
                    {
                        this._pendingSystemServices.Add(new ServiceItem(serviceType, obj2));
                    }
                }
            }
            else
            {
                this.ServiceContainer.AddService(this.SystemBundle, serviceType, serviceInstances);
            }
        }

        public void Close()
        {
        }

        public void Dispose()
        {
            if (this.EventManager != null)
            {
                this.EventManager.Dispose();
            }
        }

        public IBundle GetBundle(string location) => 
            this.Bundles.GetBundle(location);

        public IBundle GetBundleByID(int bundleID) => 
            this.Bundles.GetBundle((long) bundleID);

        public IBundle GetBundleBySymbolicName(string name) => 
            this.Bundles.GetBundleBySymbolicName(name);

        public IBundleMetadata GetBundleMetadata(long bundleID)
        {
            List<IBundleMetadata> bundles = this.FrameworkAdaptor.State.Bundles;
            return bundles?.Find(metadata => metadata.BundleID == bundleID);
        }

        protected void Initialize()
        {
            this.RegisterDefaultService();
            this.RegisterPendingServices();
            this.Bundles.Clear();
            this.Commands.Commands.Clear();
            this.Commands.NamedCommands.Clear();
            this.InitializeCommands(this.Commands);
            this.FrameworkAdaptor.Initialize();
        }

        private void InitializeCommands(GInterface0 _commands)
        {
            this.AddCommand(CommandAdaptor.CreateAdaptor<InstallCommand>());
            this.AddCommand("stop", CommandAdaptor.CreateAdaptor<StopFrameworkCommand>());
            this.AddCommand(CommandAdaptor.CreateAdaptor<StartBundleCommand>());
            this.AddCommand(CommandAdaptor.CreateAdaptor<StopBundleCommand>());
        }

        private void RegisterDefaultService()
        {
            this.ServiceContainer.AddService(this.SystemBundle, typeof(IBundleInstallerService), new object[] { new BundleInstallerService(this, this.Options.GlobalPersistentFile, this.Options.PluginsDirectoryList.ToArray()) });
            this.ServiceContainer.AddService(this.SystemBundle, typeof(IBundlePersistent), new object[] { new BundlePersistent() });
            this.ServiceContainer.AddService(this.SystemBundle, typeof(IBundleFactory), new object[] { new BundleFactory(this) });
            this.ServiceContainer.AddService(this.SystemBundle, typeof(IBundleManager), new object[] { new BundleManager(this) });
            this.ServiceContainer.AddService(this.SystemBundle, typeof(IExtensionManager), new object[] { new ExtensionManager(this) });
        }

        private void RegisterPendingServices()
        {
            this._pendingSystemServices.ForEach(item => this.ServiceContainer.AddService(this.SystemBundle, item.ServiceType, new object[] { item.ServiceInstance }));
        }

        public void RemoveCommand(ICommandAdaptor cmd)
        {
            AssertUtility.ArgumentNotNull(cmd, "cmd");
            this.Commands.Commands.Remove(cmd);
        }

        public void RemoveSystemService(Type serviceType, object serviceInstance)
        {
            Predicate<ServiceItem> match = null;
            if (this.SystemBundle == null)
            {
                if (match == null)
                {
                    match = item => (item.ServiceInstance == serviceInstance) && (item.ServiceType == serviceType);
                }
                this._pendingSystemServices.RemoveAll(match);
            }
            else
            {
                this.ServiceContainer.RemoveService(this.SystemBundle, serviceType, serviceInstance);
            }
        }

        public bool RunCommand(string cmd) => 
            this.Commands.Run(cmd);

        public void Start()
        {
            lock (this._lockObject)
            {
                if (!this.IsActive)
                {
                    this.EventManager.Start();
                    this.Initialize();
                    this.IsActive = true;
                    this.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Starting, this));
                    this.SystemBundle.Start(BundleStartOptions.Transient);
                    this.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Started, this));
                }
            }
        }

        public void Stop()
        {
            lock (this._lockObject)
            {
                if (this.IsActive)
                {
                    this.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Stopping, this));
                    string globalPersistentFile = this.ServiceContainer.GetFirstOrDefaultService<IBundleInstallerService>().GlobalPersistentFile;
                    this.ServiceContainer.GetFirstOrDefaultService<IBundlePersistent>().Save(globalPersistentFile);
                    this.SystemBundle.Stop();
                    this.Close();
                    this.IsActive = false;
                    this.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Stopped, this));
                    this.EventManager.Stop();
                    this.ServiceContainer = new ServiceManager(this);
                }
            }
        }

        public BundleRepository Bundles { get; private set; }

        public GInterface0 Commands { get; private set; }

        public EventManager EventManager { get; private set; }

        public IFrameworkAdaptor FrameworkAdaptor { get; private set; }

        public bool IsActive { get; set; }

        public FrameworkOptions Options { get; set; }

        public IServiceManager ServiceContainer { get; private set; }

        public StartLevelManager StartLevelManager { get; set; }

        public SystemBundle SystemBundle { get; set; }

        private class ServiceItem
        {
            public ServiceItem(Type serviceType, object serviceInstance)
            {
                this.ServiceType = serviceType;
                this.ServiceInstance = serviceInstance;
            }

            public object ServiceInstance { get; set; }

            public Type ServiceType { get; set; }
        }
    }
}

