namespace UIShell.OSGi.Loader
{
    using System;

    public class UseDefaultClrLoaderBehavior : IDisposable
    {
        [ThreadStatic]
        internal static bool IsStopClrExtensionLoader = true;

        [field: ThreadStatic]
        public event ResolveEventHandler AssemblyResolve;

        public void Dispose()
        {
            IsStopClrExtensionLoader = false;
            AssemblyResolve = null;
        }
    }
}

