namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;

    internal class BundleLoaderProxy
    {
        private IBundleLoader _loader;

        public BundleLoaderProxy(IBundle bundle)
        {
            Bundle = bundle;
        }

        private void CreateBundleLoader()
        {
            if (_loader == null)
            {
                _loader = BundleLoaderProxyFactory.CreateBundleLoader(Bundle, Framework);
            }
        }

        public Type LoadClass(string className)
        {
            CreateBundleLoader();
            return _loader.LoadClass(className);
        }

        public object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode)
        {
            CreateBundleLoader();
            return _loader.LoadResource(resourceName, resourceLoadMode);
        }

        public IBundle Bundle { get; set; }

        public IBundleLoader BundleLoader
        {
            get
            {
                if (_loader == null)
                {
                    CreateBundleLoader();
                }
                return _loader;
            }
        }

        public Core.Framework Framework { get; set; }
    }
}

