namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Bundle;
    using Service;
    using Dependency.Metadata;
    using Utility;

    internal class BundleManager : IBundleManager
    {
        public BundleManager(Framework framework)
        {
            Framework = framework;
        }

        public List<BundleInfo> GetAllBundles()
        {
            List<BundleInfo> rt = new List<BundleInfo>();
            Framework.Bundles.ForEach(delegate (IBundle a) {
                AbstractBundle bundle = (AbstractBundle) a;
                BundleInfo item = new BundleInfo {
                    BundleState = bundle.State,
                    BundleData = bundle.BundleData
                };
                rt.Add(item);
            });
            return rt;
        }

        public IBundle InstallBundle(string location) =>
            InstallBundle(location, null);

        public IBundle InstallBundle(string location, Stream stream)
        {
            IBundle bundle = Framework.Bundles.GetBundle(location);
            if (bundle == null)
            {
                BundleData bundleData = Framework.ServiceContainer.GetFirstOrDefaultService<IBundleInstallerService>().CreateBundleData(location, stream);
                if (bundleData == null)
                {
                    Framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Error, new Exception($"Install bundle in '{location}' failed.")));
                    return null;
                }
                Tuple<IBundle, IBundleMetadata> tuple = Framework.AddBundleData(bundleData);
                bundle = tuple.Item1;
                IBundleMetadata metadata = tuple.Item2;
                if (string.IsNullOrEmpty(bundleData.HostBundleSymbolicName))
                {
                    Interface1 host = metadata as Interface1;
                    AssertUtility.IsTrue(host != null);
                    List<IFragmentBundleMetadata> metadatas = BundleUtility.GetMetadatas<IFragmentBundleMetadata>(Framework.FrameworkAdaptor.State.Bundles);
                    BundleUtility.BindFragmentMetadatas(host, metadatas);
                }
                else
                {
                    IFragmentBundleMetadata metadata2 = metadata as IFragmentBundleMetadata;
                    AssertUtility.IsTrue(metadata2 != null);
                    BundleUtility.GetMetadatas<Interface1>(Framework.FrameworkAdaptor.State.Bundles);
                }
                Framework.ServiceContainer.GetFirstOrDefaultService<IBundlePersistent>().SaveInstallLocation(location);
            }
            return bundle;
        }

        public void Start(string bundleSymbolicName)
        {
            Start(bundleSymbolicName, BundleStartOptions.General);
        }

        public void Start(string bundleSymbolicName, BundleStartOptions option)
        {
            IBundle bundleBySymbolicName = Framework.GetBundleBySymbolicName(bundleSymbolicName);
            if (bundleBySymbolicName != null)
            {
                bundleBySymbolicName.Start(option);
            }
        }

        public void Stop(string bundleSymbolicName)
        {
            Stop(bundleSymbolicName, BundleStopOptions.General);
        }

        public void Stop(string bundleSymbolicName, BundleStopOptions option)
        {
            IBundle bundleBySymbolicName = Framework.GetBundleBySymbolicName(bundleSymbolicName);
            if (bundleBySymbolicName != null)
            {
                bundleBySymbolicName.Stop(option);
            }
        }

        public void Uninstall(string bundleSymbolicName)
        {
            IBundle bundleBySymbolicName = Framework.GetBundleBySymbolicName(bundleSymbolicName);
            if (bundleBySymbolicName != null)
            {
                bundleBySymbolicName.Stop(BundleStopOptions.General);
                bundleBySymbolicName.Uninstall();
            }
        }

        public void Update(string bundleSymbolicName)
        {
            throw new NotImplementedException();
        }

        public Framework Framework { get; private set; }
    }
}

