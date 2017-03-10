namespace UIShell.OSGi
{
    using System;

    public class ServiceEventArgs : EventArgs
    {
        internal ServiceEventArgs(string serviceType, object[] serviceInstances, ServiceEventType eventType)
        {
            if ((serviceType == null) || (serviceType.Length == 0))
            {
                throw new ArgumentException("Service type can no be null or empty.", "serviceType");
            }
            ServiceInstances = (serviceInstances == null) ? new object[0] : serviceInstances;
            ServiceType = serviceType;
            ServiceEventType = eventType;
        }

        public ServiceEventType ServiceEventType { get; private set; }

        public object[] ServiceInstances { get; private set; }

        public string ServiceType { get; private set; }
    }
}

