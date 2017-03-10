namespace UIShell.OSGi.Core.Adaptor
{
    using System;
    using System.Collections.Generic;
    using OSGi;
    using Event;

    internal class HookRegistry : FrameworkAdaptorHook
    {
        private List<FrameworkAdaptorHook> _hooks = new List<FrameworkAdaptorHook>();

        public void HandleRuntimeException(Exception exception)
        {
            foreach (var hook in _hooks)
            {
                hook.HandleRuntimeException(exception);
            }
        }

        public void Initialize(EventManager eventManager)
        {
            foreach (var hook in _hooks)
            {
                hook.Initialize(eventManager);
            }
        }

        public void OnFrameworkStart(IBundleContext context)
        {
            foreach (var hook in _hooks)
            {
                hook.OnFrameworkStart(context);
            }
        }

        public void OnFrameworkStop(IBundleContext context)
        {
            foreach (var hook in _hooks)
            {
                hook.OnFrameworkStop(context);
            }
        }

        public void OnFrameworkStopping(IBundleContext context)
        {
            foreach (var hook in _hooks)
            {
                hook.OnFrameworkStopping(context);
            }
        }

        public List<FrameworkAdaptorHook> Hooks =>
            _hooks;
    }
}

