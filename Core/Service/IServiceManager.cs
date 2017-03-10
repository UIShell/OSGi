namespace UIShell.OSGi.Core.Service
{
    using System;
    using System.Collections.Generic;
    using OSGi;

    public interface IServiceManager
    {
        void AddService<T>(IBundle owner, params object[] serviceInstances);
        void AddService(IBundle owner, object serviceInstance, params Type[] serviceTypes);
        void AddService(IBundle owner, Type serviceType, params object[] serviceInstances);
        T GetFirstOrDefaultService<T>();
        object GetFirstOrDefaultService(string serviceTypeName);
        object GetFirstOrDefaultService(Type serviceType);
        List<T> GetService<T>();
        List<object> GetService(string serviceTypeName);
        List<object> GetService(Type serviceType);
        Dictionary<Type, List<object>> GetServices();
        void RemoveService(IBundle owner, object serviceInstance);
        void RemoveService<T>(IBundle owner, object serviceInstance);
        void RemoveService(IBundle owner, Type serviceType, object serviceInstance);
    }
}

