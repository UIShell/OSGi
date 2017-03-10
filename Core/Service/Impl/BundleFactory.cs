namespace UIShell.OSGi.Core.Service.Impl
{
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Bundle;
    using Service;

    internal class BundleFactory : IBundleFactory
    {
        public BundleFactory(Framework framework)
        {
            InitialBundleId = 2;
            MaxBundleID = InitialBundleId;
            Framework = framework;
        }

        public IBundle CreateBundle(BundleData bundleData)
        {
            int num;
            var target = CreateBundle(bundleData, Framework);
            target.State = BundleState.Installed;
            target.Location = bundleData.Path;
            Framework.EventManager.DispatchFrameworkEvent(Framework, new FrameworkEventArgs(FrameworkEventType.Info, target));
            MaxBundleID = (num = MaxBundleID) + 1;
            target.BundleID = num;
            return target;
        }

        private static AbstractBundle CreateBundle(BundleData bundleData, Framework framework)
        {
            if (!string.IsNullOrEmpty(bundleData.HostBundleSymbolicName))
            {
                return new FragmentBundle(bundleData, framework);
            }
            return new HostBundle(bundleData, framework);
        }

        public Framework Framework { get; private set; }

        public int InitialBundleId { get; private set; }

        public int MaxBundleID { get; private set; }
    }
}

