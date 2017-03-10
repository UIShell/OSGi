namespace UIShell.OSGi.Core.Bundle
{
    using System;
    using System.IO;
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Service;
    using Dependency.Resolver;
    using Loader;
    using Persistent;
    using Utility;

    internal class HostBundle : AbstractBundle, IHostBundle, IPersistent
    {
        private bool _tryStarted;

        public HostBundle(BundleData bundleData, Framework framework)
            : base(bundleData, framework)
        {
        }

        public void ActivateForStarting()
        {
            if (base.State == BundleState.Starting)
            {
                this.CallTryStart();
            }
        }

        private void CallTryStart()
        {
            if (!this._tryStarted)
            {
                try
                {
                    this.StartImmdiately();
                }
                finally
                {
                    this._tryStarted = false;
                }
            }
        }

        private IBundleActivator CreateBundleActivator()
        {
            if (((base.BundleData == null) || (base.BundleData.Activator == null)) || string.IsNullOrEmpty(base.BundleData.Activator.Type))
            {
                return null;
            }
            base.ActivatorType = this.LoadClass(base.BundleData.Activator.Type);
            if (base.ActivatorType == null)
            {
                throw new Exception($"Can not load the type activator '{base.BundleData.Activator.Type}' for bundle '{base.SymbolicName}'.");
            }
            if (!typeof(IBundleActivator).IsAssignableFrom(base.ActivatorType))
            {
                throw new Exception($"Activator type '{base.BundleData.Activator.Type}' of bundle '{base.SymbolicName}' does not inherit from IBundleActivator.");
            }
            return (System.Activator.CreateInstance(base.ActivatorType) as IBundleActivator);
        }

        public override BundleLoader CreateBundleLoader() => 
            new HostBundleLoader(this);

        protected override void DoStart(BundleStartOptions option)
        {
            if (((base.State != BundleState.Starting) && (base.State != BundleState.Stopping)) && !base.IsActive)
            {
                if (!base.IsResolved)
                {
                    base.Framework.FrameworkAdaptor.State.Resolve();
                    if (!base.IsResolved)
                    {
                        throw new BundleException($"The bundle '{base.SymbolicName}, {base.Version}' is started failed since it can not be resolved.");
                    }
                }
                if (LicenseService.RequireBundleLicenseValidation)
                {
                    try
                    {
                        LicenseService.ValidateBundleLicense(base.BundleData, true);
                    }
                    catch (Exception exception)
                    {
                        FileLogUtility.Error(exception);
                        return;
                    }
                }
                if ((base.BundleData.Activator == null) || (base.BundleData.Activator.Policy != ActivatorPolicy.Lazy))
                {
                    base.State = BundleState.Starting;
                    if (option != BundleStartOptions.Transient)
                    {
                        BundlePersistentData persistentData = this.GetPersistentData();
                        if ((persistentData != null) && (persistentData.State == BundleState.Active))
                        {
                            this.CallTryStart();
                            return;
                        }
                    }
                    this.CallTryStart();
                    if (option != BundleStartOptions.Transient)
                    {
                        this.SavePersistent();
                    }
                }
                else if (base.State != BundleState.Starting)
                {
                    base.State = BundleState.Starting;
                    base.Framework.EventManager.DispatchBundleLazyActivateEvent(this, new BundleLazyActivatedEventArgs(this));
                }
            }
        }

        protected override void DoStop(BundleStopOptions option)
        {
            if (((base.State != BundleState.Starting) && (base.State != BundleState.Stopping)) && base.IsActive)
            {
                base.State = BundleState.Stopping;
                IBundleContext context = base.Context;
                Exception exception = null;
                try
                {
                    if (this.Activator != null)
                    {
                        this.Activator.Stop(context);
                    }
                }
                catch (Exception exception2)
                {
                    FileLogUtility.Error(string.Format(Messages.ExceptionOccursWhenStopping, base.SymbolicName, base.Version));
                    FileLogUtility.Error(exception2);
                    exception = exception2;
                }
                this.CheckValidState();
                base.State = BundleState.Resolved;
                if (option == BundleStopOptions.General)
                {
                    this.SavePersistent();
                }
                base.Context.Dispose();
                base.Context = null;
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        protected override void DoUninstall()
        {
            if (((base.State != BundleState.Active) && (base.State != BundleState.Starting)) && (base.State != BundleState.Stopping))
            {
                this.UnResolve();
                base.State = BundleState.Uninstalled;
                base.Framework.ServiceContainer.GetFirstOrDefaultService<IBundlePersistent>().SaveUnInstallLocation(base.Location);
            }
        }

        public DefaultBundleState? GetBundleStartState()
        {
            BundlePersistentData persistentData = this.GetPersistentData();
            if (persistentData != null)
            {
                if (persistentData.State == BundleState.Active)
                {
                    return 0;
                }
                if (persistentData.State == BundleState.Installed)
                {
                    return 1;
                }
            }
            if (base.BundleData.InitializedState.HasValue)
            {
                switch (base.BundleData.InitializedState.Value)
                {
                    case BundleInitializedState.Installed:
                        return 1;

                    case BundleInitializedState.Active:
                        return 0;
                }
            }
            return null;
        }

        protected virtual string GetDefaultPersistFile() => 
            Path.Combine(base.Location, base.Framework.Options.BundlePersistentFileName);

        private BundlePersistentData GetPersistentData() => 
            (this.Load(string.Empty) as BundlePersistentData);

        public object Load(string file)
        {
            string path = string.IsNullOrEmpty(file) ? this.GetDefaultPersistFile() : file;
            if (!File.Exists(path))
            {
                return null;
            }
            BundlePersistentData data = new BundlePersistentData();
            data.Load(path);
            if (data == null)
            {
                throw new Exception($"Load persistent file {path} fail!");
            }
            return data;
        }

        public override Type LoadClass(string className)
        {
            if (base.ActivatorPolicy == ActivatorPolicy.Lazy)
            {
                this.ActivateForStarting();
            }
            return base.LoadClass(className);
        }

        public override object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            if (base.ActivatorPolicy == ActivatorPolicy.Lazy)
            {
                this.ActivateForStarting();
            }
            return base.LoadResource(resourceName, loadMode);
        }

        private void RealseResource()
        {
        }

        public void Save(string file)
        {
            new BundlePersistentData(this).Save(string.IsNullOrEmpty(file) ? this.GetDefaultPersistFile() : file);
        }

        public void SavePersistent()
        {
            this.Save(string.Empty);
        }

        private void StartImmdiately()
        {
            this._tryStarted = true;
            if (base.Context == null)
            {
                base.Context = this.CreateBundleContext();
                try
                {
                    this.Activator = this.CreateBundleActivator();
                    if (this.Activator != null)
                    {
                        this.Activator.Start(base.Context);
                    }
                }
                catch (Exception exception)
                {
                    base.Context.Dispose();
                    base.Context = null;
                    base.State = BundleState.Stopping;
                    this.RealseResource();
                    base.State = BundleState.Resolved;
                    throw new BundleException($"The activator of bundle '{base.SymbolicName}' start failed.", exception);
                }
            }
            this.CheckValidState();
            base.State = BundleState.Active;
        }

        private void UnResolve()
        {
            Predicate<Interface2> match = null;
            if (base.BundleData != null)
            {
                if (match == null)
                {
                    match = a => a.SymbolicName == base.BundleData.SymbolicName;
                }
                Interface2 interface2 = base.Framework.FrameworkAdaptor.State.Resolver.ResolvedNodes.Find(match);
                if ((interface2 != null) && !interface2.Unresolve())
                {
                    base.Framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Error, this, new Exception($"Bundle '{base.SymbolicName}' unresolve failed.")));
                }
            }
        }

        public IBundleActivator Activator { get; private set; }
    }
}

