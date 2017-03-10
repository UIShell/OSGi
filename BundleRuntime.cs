namespace UIShell.OSGi
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Core;
    using Core.Service;
    using Utility;

    public class BundleRuntime : IDisposable
    {
        private bool _isDisposed;
        private bool _started;
        private volatile BundleRuntimeState _state;

        public int BundleClassLoaderCacheSize { get; set; }

        public bool EnableBundleClassLoaderCache { get; set; }

        public bool EnableGlobalAssemblyFeature { get; set; }

        public IFramework Framework { get; private set; }

        public static BundleRuntime Instance { get; internal set; }

        internal IServiceManager ServiceManager => Framework.ServiceContainer;

        public string[] StartArgs { get; private set; }

        public BundleRuntimeState State
        {
            get { return _state; }
            private set { _state = value; }
        }

        public BundleRuntime()
            : this(new string[] { "Plugins" })
        {
        }

        public BundleRuntime(string pluginsPath)
            : this(new string[] { pluginsPath })
        {
        }

        public BundleRuntime(string[] pluginsPathList)
        {
            BundleClassLoaderCacheSize = 50;
            if (Instance != null)
            {
                throw new Exception(Messages.SingletonOSGiAllowed);
            }
            if ((pluginsPathList == null) || (pluginsPathList.Length == 0))
            {
                throw new Exception(Messages.AtLeastOnePluginsPathMustBeSpecified);
            }
            var framework = new Framework
            {
                Options = new FrameworkOptions(pluginsPathList)
            };
            Framework = framework;
            Instance = this;
        }

        public void AddService<ServiceInterface>(object serviceInstance)
        {
            Framework.AddSystemService(typeof(ServiceInterface), new object[] { serviceInstance });
        }

        public void AddService(Type serviceInterface, object serviceInstance)
        {
            Framework.AddSystemService(serviceInterface, new object[] { serviceInstance });
        }

        public static void Dispose()
        {
            if (Instance != null)
            {
                if (Instance._started)
                {
                    Instance.Stop();
                }
                Instance = null;
            }
        }

        public T GetFirstOrDefaultService<T>()
        {
            if (ServiceManager != null)
            {
                return ServiceManager.GetFirstOrDefaultService<T>();
            }
            return default(T);
        }

        public object GetFirstOrDefaultService(string serviceTypeName)
        {
            if (ServiceManager != null)
            {
                return ServiceManager.GetFirstOrDefaultService(serviceTypeName);
            }
            return null;
        }

        public object GetFirstOrDefaultService(Type serviceType)
        {
            if (ServiceManager != null)
            {
                return ServiceManager.GetFirstOrDefaultService(serviceType);
            }
            return null;
        }

        public List<T> GetService<T>()
        {
            if (ServiceManager != null)
            {
                return ServiceManager.GetService<T>();
            }
            return null;
        }

        public List<object> GetService(string serviceTypeName)
        {
            if (ServiceManager != null)
            {
                return ServiceManager.GetService(serviceTypeName);
            }
            return null;
        }

        public List<object> GetService(Type serviceType)
        {
            if (ServiceManager != null)
            {
                return ServiceManager.GetService(serviceType);
            }
            return null;
        }

        public void RemoveService<ServiceInterface>(object serviceInstance)
        {
            Framework.RemoveSystemService(typeof(ServiceInterface), serviceInstance);
        }

        public void RemoveService(Type serviceInterface, object serviceInstance)
        {
            Framework.RemoveSystemService(serviceInterface, serviceInstance);
        }

        public void Start()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "FrameworkThread";
            }
            Start(null);
        }

        public void Start(string[] args)
        {
            if (!_started)
            {
                StartArgs = args;
                State = BundleRuntimeState.Starting;
                try
                {
                    Framework.Start();
                    State = BundleRuntimeState.Started;
                    _started = true;
                }
                catch (Exception exception)
                {
                    State = BundleRuntimeState.Stopped;
                    _started = false;
                    FileLogUtility.Error(Messages.StartTheFrameworkFailed);
                    FileLogUtility.Error(exception);
                }
            }
        }

        public void Stop()
        {
            if (_started)
            {
                State = BundleRuntimeState.Stopping;
                try
                {
                    Framework.Stop();
                }
                catch (Exception exception)
                {
                    FileLogUtility.Error(Messages.StopTheFrameworkFailed);
                    FileLogUtility.Error(exception);
                }
                State = BundleRuntimeState.Stopped;
                _started = false;
            }
        }

        void IDisposable.Dispose()
        {
            if (!_isDisposed)
            {
                Dispose();
                GC.SuppressFinalize(this);
                State = BundleRuntimeState.Disposed;
                _isDisposed = true;
            }
        }
    }
}

