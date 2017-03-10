namespace UIShell.OSGi.Core.Adaptor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Bundle;
    using Configuration.BundleManifest;
    using Core;
    using Dependency;
    using Dependency.Metadata.Impl;
    using Loader;
    using OSGi;
    using Service;
    using Utility;

    internal class FrameworkAdaptor : IFrameworkAdaptor
    {
        private Framework _framework;

        public FrameworkAdaptor(Framework framework)
        {
            if (framework == null)
            {
                throw new ArgumentNullException();
            }
            _framework = framework;
            InstalledBundles = new List<BundleData>();
            framework.EventManager.AddFrameworkEventListener(new EventHandler<FrameworkEventArgs>(EventManager_FrameworkStateChanged));
        }

        public void CompactStorage()
        {
            throw new NotImplementedException();
        }

        public void CreateSystemBundle()
        {
            _framework.SystemBundle = new SystemBundle(_framework);
            _framework.SystemBundle.BundleID = 1L;
        }

        private void EventManager_FrameworkStateChanged(object sender, FrameworkEventArgs e)
        {
            var target = e.Target as IBundleContext;
            if (target == null)
            {
                var bundle = e.Target as IBundle;
                if (bundle != null)
                {
                    target = bundle.Context;
                }
            }
            switch (e.EventType)
            {
                case FrameworkEventType.Starting:
                    OnFrameworkStarting();
                    return;

                case FrameworkEventType.Started:
                    OnFrameworkStarted();
                    return;

                case FrameworkEventType.Error:
                    if (e.Data != null)
                    {
                        if (!(e.Data is Exception))
                        {
                            break;
                        }
                        FileLogUtility.Error(e.Data as Exception);
                        return;
                    }
                    if (target != null)
                    {
                        FileLogUtility.Error(string.Format(Messages.BundleThrownFrameworkError, target.Bundle.SymbolicName, target.Bundle.Version));
                        return;
                    }
                    FileLogUtility.Error(string.Format(Messages.ObjectThrownFrameworkError, e.Target));
                    return;

                case FrameworkEventType.Warning:
                    if (e.Data != null)
                    {
                        if (e.Data is Exception)
                        {
                            FileLogUtility.Warn(e.Data as Exception);
                            return;
                        }
                        break;
                    }
                    if (target != null)
                    {
                        FileLogUtility.Warn(string.Format(Messages.BundleThrownFrameworkWarning, target.Bundle.SymbolicName, target.Bundle.Version));
                        return;
                    }
                    FileLogUtility.Warn(string.Format(Messages.ObjectThrownFrameworkWarning, e.Target));
                    return;

                case FrameworkEventType.Info:
                case FrameworkEventType.Stopping:
                    break;

                case FrameworkEventType.Stopped:
                    OnFrameworkStop();
                    break;

                default:
                    return;
            }
        }

        public void Initialize()
        {
            State = new Dependency.Impl.State();
            State.MetadataBuilder = new MetadataBuilder(_framework);
            var firstOrDefaultService = _framework.ServiceContainer.GetFirstOrDefaultService<IBundleInstallerService>();
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                if (!firstOrDefaultService.InstallBundles())
                {
                    throw new Exception(Messages.LoadBundlesFailed);
                }
            }
            finally
            {
                stopwatch.Stop();
                FileLogUtility.Verbose(string.Format(Messages.InstallBundlesTimeCounter, stopwatch.ElapsedMilliseconds));
            }
            UpdateService.Update(new List<BundleData>(firstOrDefaultService.BundleDatas.Values), firstOrDefaultService.UpdateFolder);
            LicenseService.EnsureHasAvailableBundleLicense();
            var metadataBuilder = State.MetadataBuilder;
            CreateSystemBundle();
            foreach (KeyValuePair<string, BundleData> pair in firstOrDefaultService.BundleDatas)
            {
                _framework.AddBundleData(pair.Value);
            }
        }

        public void InitializeStorage()
        {
            throw new NotImplementedException();
        }

        public void OnFrameworkStarted()
        {
        }

        public void OnFrameworkStarting()
        {
            var provider = new AssemblyResolvingProvider(this, _framework);
            _framework.ServiceContainer.AddService(_framework.SystemBundle, typeof(IAssemblyResolvingProvider), new object[] { provider });
            _framework.ServiceContainer.AddService(_framework.SystemBundle, typeof(IRuntimeService), new object[] { provider });
            provider.Start();
        }

        public void OnFrameworkStop()
        {
            var firstOrDefaultService = _framework.ServiceContainer.GetFirstOrDefaultService<IAssemblyResolvingProvider>();
            if (firstOrDefaultService != null)
            {
                firstOrDefaultService.Stop();
            }
        }

        public List<BundleData> InstalledBundles { get; private set; }

        public IState State { get; set; }
    }
}

