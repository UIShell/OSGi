namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;

    internal interface IRuntimeLoader
    {
        Type LoadClass(string className);
        object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode);

        IBundleLoader Owner { get; set; }
    }
}

