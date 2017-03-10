namespace UIShell.OSGi.Core.Bundle
{
    using System.Collections.Generic;
    using OSGi;
    using Core;
    using Service;

    public class BundleRepository : List<IBundle>
    {
        private List<IBundle> _bundles;
        private IFramework _framework;

        public BundleRepository(IFramework framework)
        {
            _bundles = this;
            _framework = framework;
        }

        public IBundle GetBundle(long bundleID) =>
            _bundles.Find(bundle => bundle.BundleID == bundleID);

        public IBundle GetBundle(string location) =>
            _bundles.Find(bundle => bundle.Location.CompareTo(location) == 0);

        public IBundle GetBundleBySymbolicName(string name)
        {
            string bundlePath = _framework.ServiceContainer.GetFirstOrDefaultService<IBundleInstallerService>().GetBundlePath(name);
            if (string.IsNullOrEmpty(bundlePath))
            {
                return null;
            }
            return GetBundle(bundlePath);
        }

        public IBundle RemoveBundle(long bundleID)
        {
            IBundle item = GetBundle(bundleID);
            if (item != null)
            {
                _bundles.Remove(item);
            }
            return item;
        }

        public IBundle RemoveBundle(string location)
        {
            IBundle item = GetBundle(location);
            if (item != null)
            {
                _bundles.Remove(item);
            }
            return item;
        }
    }
}

