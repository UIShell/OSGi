namespace UIShell.OSGi
{
    public interface IBundleActivator
    {
        void Start(IBundleContext context);
        void Stop(IBundleContext context);
    }
}

