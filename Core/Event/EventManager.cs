namespace UIShell.OSGi.Core.Event
{
    using System;
    using System.Collections.Generic;
    using OSGi;

    public class EventManager : IDisposable
    {
        private EventDispatcher<BundleStateChangedEventArgs> _bundleDispatcher;
        private EventDispatcher<BundleLazyActivatedEventArgs> _bundleLazyDispatcher;
        private List<EventHandler<BundleLazyActivatedEventArgs>> _bundleLazyListeners = new List<EventHandler<BundleLazyActivatedEventArgs>>();
        private List<EventHandler<BundleStateChangedEventArgs>> _bundleListeners = new List<EventHandler<BundleStateChangedEventArgs>>();
        private EventDispatcher<FrameworkEventArgs> _frameworkDispatcher;
        private List<EventHandler<FrameworkEventArgs>> _frameworkListeners = new List<EventHandler<FrameworkEventArgs>>();
        private EventDispatcher<ServiceEventArgs> _serviceDispatcher;
        private List<EventHandler<ServiceEventArgs>> _serviceListeners = new List<EventHandler<ServiceEventArgs>>();
        private List<EventHandler<BundleStateChangedEventArgs>> _syncBundleListeners = new List<EventHandler<BundleStateChangedEventArgs>>();

        public void AddBundleEventListener(EventHandler<BundleStateChangedEventArgs> handler, bool sync)
        {
            if (sync)
            {
                _syncBundleListeners.Add(handler);
            }
            else
            {
                _bundleListeners.Add(handler);
            }
        }

        public void AddBundleLazyActivatedEventListener(EventHandler<BundleLazyActivatedEventArgs> handler)
        {
            _bundleLazyListeners.Add(handler);
        }

        public void AddFrameworkEventListener(EventHandler<FrameworkEventArgs> handler)
        {
            _frameworkListeners.Add(handler);
        }

        public void AddServiceEventListener(EventHandler<ServiceEventArgs> handler)
        {
            _serviceListeners.Add(handler);
        }

        public void DispatchBundleEvent(object sender, BundleStateChangedEventArgs e)
        {
            if (_bundleDispatcher != null)
            {
                List<EventHandler<BundleStateChangedEventArgs>> listeners = new List<EventHandler<BundleStateChangedEventArgs>>(_syncBundleListeners);
                List<EventHandler<BundleStateChangedEventArgs>> list2 = new List<EventHandler<BundleStateChangedEventArgs>>(_bundleListeners);
                if ((e.CurrentState != BundleState.Starting) && (e.CurrentState != BundleState.Stopping))
                {
                    _bundleDispatcher.DispatchToAsyncListeners(sender, e, list2);
                }
                _bundleDispatcher.DispatchToSyncListeners(sender, e, listeners);
            }
        }

        public void DispatchBundleLazyActivateEvent(object sender, BundleLazyActivatedEventArgs e)
        {
            if (_bundleLazyDispatcher != null)
            {
                List<EventHandler<BundleLazyActivatedEventArgs>> listeners = new List<EventHandler<BundleLazyActivatedEventArgs>>(_bundleLazyListeners);
                _bundleLazyDispatcher.DispatchToSyncListeners(sender, e, listeners);
            }
        }

        public void DispatchFrameworkEvent(object sender, FrameworkEventArgs e)
        {
            if (_frameworkDispatcher != null)
            {
                List<EventHandler<FrameworkEventArgs>> listeners = new List<EventHandler<FrameworkEventArgs>>(_frameworkListeners);
                if (e.EventType == FrameworkEventType.Stopping)
                {
                    _frameworkDispatcher.DispatchToSyncListeners(sender, e, listeners);
                }
                else
                {
                    _frameworkDispatcher.DispatchToAsyncListeners(sender, e, listeners);
                }
            }
        }

        public void DispatchServiceEvent(object sender, ServiceEventArgs e)
        {
            if (_serviceDispatcher != null)
            {
                List<EventHandler<ServiceEventArgs>> listeners = new List<EventHandler<ServiceEventArgs>>(_serviceListeners);
                _serviceDispatcher.DispatchToAsyncListeners(sender, e, listeners);
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public void RemoveBundleEventListener(EventHandler<BundleStateChangedEventArgs> handler, bool sync)
        {
            if (sync)
            {
                _syncBundleListeners.Remove(handler);
            }
            else
            {
                _bundleListeners.Remove(handler);
            }
        }

        public void RemovedBundleLazyActivatedEventListener(EventHandler<BundleLazyActivatedEventArgs> handler)
        {
            _bundleLazyListeners.Remove(handler);
        }

        public void RemoveFrameworkEventListener(EventHandler<FrameworkEventArgs> handler)
        {
            _frameworkListeners.Remove(handler);
        }

        public void RemoveServiceEventListener(EventHandler<ServiceEventArgs> handler)
        {
            _serviceListeners.Remove(handler);
        }

        public void Start()
        {
            _frameworkDispatcher = new EventDispatcher<FrameworkEventArgs>(this);
            _bundleDispatcher = new EventDispatcher<BundleStateChangedEventArgs>(this);
            _serviceDispatcher = new EventDispatcher<ServiceEventArgs>(this);
            _bundleLazyDispatcher = new EventDispatcher<BundleLazyActivatedEventArgs>(this);
        }

        public void Stop()
        {
            if (_frameworkListeners != null)
            {
                _frameworkListeners.Clear();
            }
            if (_bundleListeners != null)
            {
                _bundleListeners.Clear();
            }
            if (_syncBundleListeners != null)
            {
                _syncBundleListeners.Clear();
            }
            if (_serviceListeners != null)
            {
                _serviceListeners.Clear();
            }
            if (_frameworkDispatcher != null)
            {
                _frameworkDispatcher.Dispose();
                _frameworkDispatcher = null;
            }
            if (_bundleDispatcher != null)
            {
                _bundleDispatcher.Dispose();
                _bundleDispatcher = null;
            }
            if (_bundleLazyDispatcher != null)
            {
                _bundleLazyDispatcher.Dispose();
                _bundleLazyDispatcher = null;
            }
            if (_serviceDispatcher != null)
            {
                _serviceDispatcher.Dispose();
                _serviceDispatcher = null;
            }
        }

        public IEnumerable<EventHandler<BundleStateChangedEventArgs>> BundleEventListeners =>
            _bundleListeners;

        public IEnumerable<EventHandler<FrameworkEventArgs>> FrameworkEventListeners =>
            _frameworkListeners;

        public IEnumerable<EventHandler<BundleStateChangedEventArgs>> SyncBundleEventListeners =>
            _syncBundleListeners;
    }
}

