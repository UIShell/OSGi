namespace UIShell.OSGi.Core
{
    using OSGi;

    internal interface IHostBundle
    {
        void ActivateForStarting();

        IBundleActivator Activator { get; }
    }
}

