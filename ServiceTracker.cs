namespace UIShell.OSGi
{
    using System;
    using System.Collections.Generic;
    using Utility;

    public class ServiceTracker<TServiceInterface> : IDisposable
    {
        private TServiceInterface _defaultOrFirstService;
        private List<TServiceInterface> _serviceInstances;
        private bool _throwsExceptionIfServiceNotFound;

        public ServiceTracker(IBundleContext context)
            : this(context, true)
        {
        }

        public ServiceTracker(IBundleContext context, bool throwsExceptionIfServiceNotFound)
        {
            AssertUtility.ArgumentNotNull(context, "BundleContext");
            BundleContext = context;
            _defaultOrFirstService = context.GetFirstOrDefaultService<TServiceInterface>();
            _serviceInstances = context.GetService<TServiceInterface>();
            context.ServiceChanged += new EventHandler<ServiceEventArgs>(ServiceChanged);
        }

        public void Dispose()
        {
            BundleContext.ServiceChanged -= new EventHandler<ServiceEventArgs>(ServiceChanged);
            BundleContext = null;
            _defaultOrFirstService = default(TServiceInterface);
            _serviceInstances = null;
        }

        private void ServiceChanged(object sender, ServiceEventArgs e)
        {
            if (e.ServiceType.Equals(typeof(TServiceInterface).FullName))
            {
                try
                {
                    _defaultOrFirstService = BundleContext.GetFirstOrDefaultService<TServiceInterface>();
                    _serviceInstances = BundleContext.GetService<TServiceInterface>();
                }
                catch (Exception exception)
                {
                    FileLogUtility.Error(string.Format(Messages.GetServiceFailed, typeof(TServiceInterface).FullName));
                    FileLogUtility.Error(exception);
                    _defaultOrFirstService = default(TServiceInterface);
                    _serviceInstances = null;
                }
            }
        }

        public IBundleContext BundleContext { get; private set; }

        public TServiceInterface DefaultOrFirstService
        {
            get
            {
                if ((_defaultOrFirstService == null) && _throwsExceptionIfServiceNotFound)
                {
                    throw new ServiceNotAvailableException(typeof(TServiceInterface).FullName, BundleContext.Bundle);
                }
                return _defaultOrFirstService;
            }
        }

        public bool IsServiceAvailable =>
            ((_serviceInstances != null) && (_serviceInstances.Count > 0));

        public List<TServiceInterface> ServiceInstances
        {
            get
            {
                if (_throwsExceptionIfServiceNotFound && ((_serviceInstances == null) || (_serviceInstances.Count == 0)))
                {
                    throw new ServiceNotAvailableException(typeof(TServiceInterface).FullName, BundleContext.Bundle);
                }
                return _serviceInstances;
            }
        }
    }
}

