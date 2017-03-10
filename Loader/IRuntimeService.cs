namespace UIShell.OSGi.Loader
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface IRuntimeService
    {
        List<Assembly> LoadBundleAssembly(string bundleSymbolicName);
    }
}

