namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using OSGi;
    using Collection;
    using Collection.Locker;
    using Configuration.BundleManifest;
    using Core;
    using Service;
    using Utility;

    internal class ServiceManager : IServiceManager
    {
        private IFramework _framework;
        private ServiceCollection _serviceCollection = new ServiceCollection();

        public ServiceManager(IFramework framework)
        {
            this._framework = framework;
            AssertUtility.ArgumentNotNull(framework, "framework");
            framework.EventManager.AddBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(this.BundleEventListener), true);
        }

        public void AddService<T>(IBundle owner, params object[] serviceInstance)
        {
            this.AddService(owner, typeof(T), serviceInstance);
        }

        public void AddService(IBundle owner, object serviceInstance, params Type[] serviceTypes)
        {
            foreach (Type type in serviceTypes)
            {
                this.GetOrCreateInstanceCollection(type).Add(owner, serviceInstance);
            }
        }

        public void AddService(IBundle owner, Type serviceType, params object[] serviceInstance)
        {
            foreach (object obj2 in serviceInstance)
            {
                if (!serviceType.IsAssignableFrom(obj2.GetType()))
                {
                    throw new Exception($"The {obj2} must be of type {serviceType}");
                }
            }
            this.GetOrCreateInstanceCollection(serviceType).AddRange(owner, serviceInstance);
            this._framework.EventManager.DispatchServiceEvent(this, new ServiceEventArgs(serviceType.FullName, serviceInstance, ServiceEventType.Add));
        }

        private void AddService(IBundle owner, ServiceData serviceData, Func<string, Type> loadClass)
        {
            Func<FindServiceResult, bool> func2 = null;
            Func<string, object> createServiceInstance;
            if (serviceData.Interfaces != null)
            {
                createServiceInstance = delegate (string classType) {
                    object obj2;
                    Type type = loadClass(classType);
                    if (type == null)
                    {
                        throw new Exception($"Can not load service class type:{classType}");
                    }
                    try
                    {
                        obj2 = Activator.CreateInstance(type);
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Can not create service instance for type:{classType}");
                    }
                    return obj2;
                };
                Func<InterfaceHolder> creator = null;
                foreach (string item in serviceData.Interfaces)
                {
                    if (creator == null)
                    {
                        creator = () => new InterfaceHolder(item, loadClass);
                    }
                    if (func2 == null)
                    {
                        func2 = delegate (FindServiceResult result) {
                            result.Value.AddClass(owner, serviceData.Type, createServiceInstance);
                            return true;
                        };
                    }
                    this._serviceCollection.Find<bool>(item, creator, func2);
                    this._framework.EventManager.DispatchServiceEvent(this, new ServiceEventArgs(item, null, ServiceEventType.Add));
                }
            }
        }

        private void BundleEventListener(object sender, BundleStateChangedEventArgs e)
        {
            Action<ServiceData> action = null;
            IBundle bundle;
            Func<string, Type> loadClass;
            bool flag = e.CurrentState == BundleState.Starting;
            BundleData bundleDataByName = this._framework.ServiceContainer.GetFirstOrDefaultService<IBundleInstallerService>().GetBundleDataByName(e.Bundle.SymbolicName);
            if (bundleDataByName != null)
            {
                bundle = this._framework.GetBundleBySymbolicName(bundleDataByName.SymbolicName);
                AssertUtility.NotNull(bundle);
                loadClass = classType => bundle.LoadClass(classType);
                if (flag)
                {
                    if (action == null)
                    {
                        action = serviceData => this.AddService(bundle, serviceData, loadClass);
                    }
                    bundleDataByName.Services.ForEach(action);
                }
                else if (e.CurrentState == BundleState.Stopping)
                {
                    this.RemoveServiceByOwner(bundle);
                }
            }
        }

        public T GetFirstOrDefaultService<T>()
        {
            object firstOrDefaultService = this.GetFirstOrDefaultService(typeof(T));
            if (firstOrDefaultService == null)
            {
                return default(T);
            }
            return (T) firstOrDefaultService;
        }

        public object GetFirstOrDefaultService(string serviceType)
        {
            List<object> service = this.GetService(serviceType);
            if (service.Count > 0)
            {
                return service[0];
            }
            return null;
        }

        public object GetFirstOrDefaultService(Type serviceType)
        {
            ServiceInstancesHolder orCreateInstanceCollection = this.GetOrCreateInstanceCollection(serviceType);
            if (orCreateInstanceCollection != null)
            {
                List<object> serviceInstances = orCreateInstanceCollection.GetServiceInstances();
                if (serviceInstances.Count > 0)
                {
                    return serviceInstances[0];
                }
            }
            return null;
        }

        private ServiceInstancesHolder GetOrCreateInstanceCollection(Type serviceType) => 
            this._serviceCollection.Find<ServiceInstancesHolder>(serviceType, true, result => result.Value);

        private ServiceInstancesHolder GetOrCreateInstanceCollection(string serviceType, Func<string, Type> classLoader) => 
            this._serviceCollection.Find<ServiceInstancesHolder>(serviceType, () => new InterfaceHolder(classLoader(serviceType)), result => result.Value);

        public List<T> GetService<T>() => 
            this.GetService(typeof(T))?.ConvertAll<T>(item => (T) item);

        public List<object> GetService(string serviceType)
        {
            ServiceInstancesHolder holder = this._serviceCollection.Find<ServiceInstancesHolder>(serviceType, null, result => result.Value);
            if (holder != null)
            {
                return new List<object>(holder.GetServiceInstances());
            }
            return new List<object>();
        }

        public List<object> GetService(Type serviceType)
        {
            ServiceInstancesHolder orCreateInstanceCollection = this.GetOrCreateInstanceCollection(serviceType);
            if (orCreateInstanceCollection == null)
            {
                return new List<object>();
            }
            return new List<object>(orCreateInstanceCollection.GetServiceInstances());
        }

        public Dictionary<Type, List<object>> GetServices()
        {
            Dictionary<Type, List<object>> services = new Dictionary<Type, List<object>>();
            this._serviceCollection.ForEach(delegate (KeyValuePair<InterfaceHolder, ServiceInstancesHolder> item) {
                Type key = item.Key.GetServiceType();
                List<object> serviceInstances = item.Value.GetServiceInstances();
                if (serviceInstances.Count > 0)
                {
                    services.Add(key, serviceInstances);
                }
            });
            return services;
        }

        public void RemoveService(IBundle owner, object serviceInstance)
        {
            List<Type> list = this._serviceCollection.RemoveServiceInstance(owner, serviceInstance);
            if (list.Count != 0)
            {
                foreach (Type type in list)
                {
                    this._framework.EventManager.DispatchServiceEvent(this, new ServiceEventArgs(type.FullName, new object[] { serviceInstance }, ServiceEventType.Remove));
                }
            }
        }

        public void RemoveService<T>(IBundle owner, object serviceInstance)
        {
            this.RemoveService(owner, typeof(T), serviceInstance);
        }

        public void RemoveService(IBundle owner, Type serviceType, object serviceInstance)
        {
            if (this._serviceCollection.RemoveServiceInstance(owner, serviceType, serviceInstance) != 0)
            {
                this._framework.EventManager.DispatchServiceEvent(this, new ServiceEventArgs(serviceType.FullName, new object[] { serviceInstance }, ServiceEventType.Remove));
            }
        }

        public void RemoveServiceByOwner(IBundle owner)
        {
            this._serviceCollection.ForEach(delegate (KeyValuePair<InterfaceHolder, ServiceInstancesHolder> item) {
                List<object> list = item.Value.RemoveAll(owner);
                if (list.Count > 0)
                {
                    this._framework.EventManager.DispatchServiceEvent(this, new ServiceEventArgs(item.Key.GetServiceType().FullName, list.ToArray(), ServiceEventType.Remove));
                }
            });
        }

        private class FindServiceResult
        {
            public FindServiceResult(Dictionary<ServiceManager.InterfaceHolder, ServiceManager.ServiceInstancesHolder> container, ServiceManager.InterfaceHolder key)
            {
                this.Container = container;
                this.Key = key;
                this.Value = container[key];
            }

            public Dictionary<ServiceManager.InterfaceHolder, ServiceManager.ServiceInstancesHolder> Container { get; private set; }

            public ServiceManager.InterfaceHolder Key { get; private set; }

            public ServiceManager.ServiceInstancesHolder Value { get; private set; }
        }

        public class InterfaceHolder
        {
            private ServiceManager.ObjectCreator<Type> _creator;

            public InterfaceHolder(Type type)
            {
                this._creator = new ServiceManager.ObjectCreator<Type>(null, type);
            }

            public InterfaceHolder(string serviceType, Func<string, Type> creator)
            {
                this._creator = new ServiceManager.ObjectCreator<Type>(null, serviceType, creator);
            }

            public Type GetServiceType() => 
                this.GetServiceType(true);

            public Type GetServiceType(bool allowCreate)
            {
                if (this._creator != null)
                {
                    return this._creator.GetOrCreateInstance();
                }
                return null;
            }

            public bool Match(string serviceType)
            {
                if (this._creator.Class == serviceType)
                {
                    return true;
                }
                Type orCreateInstance = this._creator.GetOrCreateInstance();
                return ((orCreateInstance != null) && (orCreateInstance.FullName == serviceType));
            }

            public bool Match(Type serviceType) => 
                ((serviceType.FullName == this._creator.Class) || (this._creator.GetOrCreateInstance() == serviceType));

            public override string ToString() => 
                this._creator.ToString();
        }

        public class ObjectCreator<T>
        {
            private T _serviceInstance;

            public ObjectCreator(IBundle owner, T instance)
            {
                AssertUtility.ArgumentNotNull(instance, "defaultValue");
                this._serviceInstance = instance;
                this.Owner = owner;
            }

            public ObjectCreator(IBundle owner, string classType, Func<string, T> creator)
            {
                AssertUtility.ArgumentNotNull(classType, "classType");
                AssertUtility.ArgumentNotNull(creator, "creator");
                this.Class = classType;
                this.Creator = creator;
                this.Owner = owner;
            }

            public T GetOrCreateInstance() => 
                this.GetOrCreateInstance(true);

            public T GetOrCreateInstance(bool allowCreate)
            {
                if (allowCreate && (this._serviceInstance == null))
                {
                    this._serviceInstance = this.Creator(this.Class);
                }
                return this._serviceInstance;
            }

            public override string ToString()
            {
                if (this._serviceInstance != null)
                {
                    return this._serviceInstance.ToString();
                }
                return this.Class;
            }

            public string Class { get; private set; }

            public Func<string, T> Creator { get; private set; }

            public IBundle Owner { get; private set; }
        }

        private class ServiceCollection : ThreadSafeDictionary<ServiceManager.InterfaceHolder, ServiceManager.ServiceInstancesHolder>
        {
            public T Find<T>(string serviceType, Func<ServiceManager.InterfaceHolder> creator, Func<ServiceManager.FindServiceResult, T> func) => 
                this.Find<T>(item => item.Match(serviceType), creator, func);

            public T Find<T>(Type serviceType, bool createIfNotFound, Func<ServiceManager.FindServiceResult, T> func)
            {
                Func<ServiceManager.InterfaceHolder> func3 = null;
                Func<ServiceManager.InterfaceHolder> creator = null;
                if (createIfNotFound)
                {
                    if (func3 == null)
                    {
                        func3 = () => new ServiceManager.InterfaceHolder(serviceType);
                    }
                    creator = func3;
                }
                return this.Find<T>(item => item.Match(serviceType), creator, func);
            }

            private T Find<T>(Func<ServiceManager.InterfaceHolder, bool> comparer, Func<ServiceManager.InterfaceHolder> creator, Func<ServiceManager.FindServiceResult, T> func)
            {
                using (DictionaryLocker<ServiceManager.InterfaceHolder, ServiceManager.ServiceInstancesHolder> locker = this.CreateLocker())
                {
                    foreach (KeyValuePair<ServiceManager.InterfaceHolder, ServiceManager.ServiceInstancesHolder> pair in locker)
                    {
                        if (comparer(pair.Key) && (func != null))
                        {
                            return func(new ServiceManager.FindServiceResult(base.Container, pair.Key));
                        }
                    }
                    if (creator != null)
                    {
                        AssertUtility.ArgumentNotNull(creator, "creator");
                        ServiceManager.InterfaceHolder key = creator();
                        locker[key] = new ServiceManager.ServiceInstancesHolder();
                        if (func != null)
                        {
                            return func(new ServiceManager.FindServiceResult(base.Container, key));
                        }
                    }
                    return default(T);
                }
            }

            public List<Type> RemoveServiceInstance(IBundle owner, object serviceInstance)
            {
                List<Type> serviceInterfaces = new List<Type>();
                base.ForEach(delegate (KeyValuePair<ServiceManager.InterfaceHolder, ServiceManager.ServiceInstancesHolder> item) {
                    if (item.Value.Remove(owner, serviceInstance) > 0)
                    {
                        serviceInterfaces.Add(item.Key.GetServiceType());
                    }
                });
                return serviceInterfaces;
            }

            public int RemoveServiceInstance(IBundle owner, Type serviceType) => 
                this.Find<int>(serviceType, false, result => result.Value.RemoveAll(item => item.Owner == owner));

            public int RemoveServiceInstance(IBundle owner, Type serviceType, object serviceInstance) => 
                this.Find<int>(serviceType, false, result => result.Value.Remove(owner, serviceInstance));
        }

        public class ServiceInstancesHolder
        {
            private ThreadSafeList<ServiceManager.ObjectCreator<object>> ServiceCreators = new ThreadSafeList<ServiceManager.ObjectCreator<object>>();

            public void Add(IBundle owner, object serviceInstance)
            {
                this.ServiceCreators.Add(new ServiceManager.ObjectCreator<object>(owner, serviceInstance));
            }

            public void AddClass(IBundle owner, string classType, Func<string, object> creator)
            {
                this.ServiceCreators.Add(new ServiceManager.ObjectCreator<object>(owner, classType, creator));
            }

            public void AddRange(IBundle owner, IEnumerable<object> serviceInstance)
            {
                foreach (object obj2 in serviceInstance)
                {
                    this.Add(owner, obj2);
                }
            }

            public List<object> GetServiceInstances() => 
                this.GetServiceInstances(true);

            public List<object> GetServiceInstances(bool allowCreate)
            {
                List<object> result = new List<object>();
                this.ServiceCreators.ForEach(delegate (ServiceManager.ObjectCreator<object> item) {
                    if (item.GetOrCreateInstance(allowCreate) != null)
                    {
                        result.Add(item.GetOrCreateInstance(allowCreate));
                    }
                });
                return result;
            }

            public int Remove(object owner, object serviceInstance) => 
                this.RemoveAll(item => (item.Owner == owner) && (item.GetOrCreateInstance(false) == serviceInstance));

            public int RemoveAll(Predicate<ServiceManager.ObjectCreator<object>> match) => 
                this.ServiceCreators.RemoveAll(match);

            public List<object> RemoveAll(IBundle owner)
            {
                List<object> result = new List<object>();
                this.ServiceCreators.RemoveAll(delegate (ServiceManager.ObjectCreator<object> item) {
                    if (item.Owner != owner)
                    {
                        return false;
                    }
                    object orCreateInstance = item.GetOrCreateInstance(false);
                    if (orCreateInstance != null)
                    {
                        result.Add(orCreateInstance);
                    }
                    return true;
                });
                return result;
            }

            public override string ToString() => 
                this.ServiceCreators.ToString();
        }
    }
}

