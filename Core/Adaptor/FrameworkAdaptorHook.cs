namespace UIShell.OSGi.Core.Adaptor
{
    using System;
    using OSGi;
    using Event;

    internal interface FrameworkAdaptorHook
    {
        void HandleRuntimeException(Exception exception);
        void Initialize(EventManager eventManager);
        void OnFrameworkStart(IBundleContext context);
        void OnFrameworkStop(IBundleContext context);
        void OnFrameworkStopping(IBundleContext context);
    }
}

