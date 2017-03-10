namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;

    internal abstract class AbstractRuntimeLoader : IRuntimeLoader
    {
        public AbstractRuntimeLoader()
        {
        }

        public AbstractRuntimeLoader(IBundleLoader ownerLoader)
        {
            Owner = ownerLoader;
        }

        public abstract Type LoadClass(string className);
        public abstract object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode);

        public IBundleLoader Owner { get; set; }
    }
}

