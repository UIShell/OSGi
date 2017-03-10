namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using Bundle;
    using Service;

    internal class DefaultBundleSortStrategy : IDefaultBundleSortStrategy
    {
        private bool HasDependentBundleInCollection(HostBundle bundle, List<HostBundle> bundlesToCheck)
        {
            if (bundle.Metadata.DependentBundles.Count == 0)
            {
                return false;
            }
            return bundle.Metadata.DependentBundles.Exists(dependent => bundlesToCheck.Exists(item => item.Metadata == dependent.DependentMetadata));
        }

        public void Sort(List<HostBundle> bundles)
        {
            var collection = new List<HostBundle>(bundles.Count);
            var list2 = new List<HostBundle>();
            int? nullable = null;
            foreach (var bundle in bundles)
            {
                if (!nullable.HasValue)
                {
                    nullable = new int?(bundle.GetBunldeStartLevel());
                    list2.Add(bundle);
                }
                else
                {
                    int bunldeStartLevel = bundle.GetBunldeStartLevel();
                    if (bunldeStartLevel == nullable)
                    {
                        list2.Add(bundle);
                    }
                    else
                    {
                        SortBundlesByDependency(list2);
                        collection.AddRange(list2);
                        list2.Clear();
                        list2.Add(bundle);
                        nullable = new int?(bundle.GetBunldeStartLevel());
                    }
                }
            }
            SortBundlesByDependency(list2);
            collection.AddRange(list2);
            bundles.Clear();
            bundles.AddRange(collection);
        }

        private void SortBundlesByDependency(List<HostBundle> bundles)
        {
            Predicate<HostBundle> match = null;
            List<HostBundle> temp = new List<HostBundle>(bundles.Count);
            while (bundles.Count > 0)
            {
                if (match == null)
                {
                    match = delegate (HostBundle bundle) {
                        if (!HasDependentBundleInCollection(bundle, bundles))
                        {
                            temp.Add(bundle);
                            return true;
                        }
                        return false;
                    };
                }
                if ((bundles.RemoveAll(match) == 0) && (bundles.Count > 0))
                {
                    break;
                }
            }
            bundles.AddRange(temp);
        }
    }
}

