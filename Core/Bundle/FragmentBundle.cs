namespace UIShell.OSGi.Core.Bundle
{
    using System;
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Loader;
    using Utility;
    using Service;

    internal class FragmentBundle : AbstractBundle
    {
        public FragmentBundle(BundleData bundleData, Framework framework)
            : base(bundleData, framework)
        {
        }

        public override BundleLoader CreateBundleLoader() => 
            new FragementBundleLoader(this);

        protected override void DoStart(BundleStartOptions option)
        {
            throw new BundleException(Messages.FragmentBundleCanNotBeStarted);
        }

        protected override void DoStop(BundleStopOptions option)
        {
            throw new BundleException(Messages.FragmentBundleCanNotBeStopped);
        }

        protected override void DoUninstall()
        {
            Framework.ServiceContainer.GetFirstOrDefaultService<IBundlePersistent>().SaveUnInstallLocation(base.Location);
        }

        public override Type LoadClass(string className)
        {
            CheckValidState();
            throw new TypeLoadException(Messages.CanNotLoadClassFromFragmentBundle);
        }

        internal Type LoadClassForHost(string className) => 
            base.LoadClass(className);

        public override object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            CheckValidState();
            return null;
        }
    }
}

