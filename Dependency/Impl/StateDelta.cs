namespace UIShell.OSGi.Dependency.Impl
{
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Utility;

    internal class StateDelta : IStateDelta
    {
        private List<IBundleDelta> _changes = new List<IBundleDelta>();

        public IBundleDelta[] GetChanges(BundleDeltaType deltaType, bool exact)
        {
            List<IBundleDelta> list = new List<IBundleDelta>();
            for (int i = 0; i < this._changes.Count; i++)
            {
                if (!exact)
                {
                    if ((this._changes[i].Type & deltaType) != BundleDeltaType.Added)
                    {
                        list.Add(this._changes[i]);
                    }
                }
                else if (this._changes[i].Type == deltaType)
                {
                    list.Add(this._changes[i]);
                }
            }
            if (list.Count != 0)
            {
                return list.ToArray();
            }
            return null;
        }

        public void RecordBundleAdded(IBundleMetadata bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            this._changes.Add(new BundleDelta(bundle, BundleDeltaType.Added));
        }

        public void RecordBundleRemovalPending(IBundleMetadata bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            this._changes.Add(new BundleDelta(bundle, BundleDeltaType.RemovalPending));
        }

        public void RecordBundleRemoved(IBundleMetadata bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            this._changes.Add(new BundleDelta(bundle, BundleDeltaType.Removed));
        }

        public void RecordBundleResolved(IBundleMetadata bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            this._changes.Add(new BundleDelta(bundle, BundleDeltaType.Resolved));
        }

        public void RecordBundleUnresolved(IBundleMetadata bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            this._changes.Add(new BundleDelta(bundle, BundleDeltaType.Unresolved));
        }

        public IBundleDelta[] Changes
        {
            get
            {
                if (this._changes.Count != 0)
                {
                    return this._changes.ToArray();
                }
                return null;
            }
        }

        public IState State { get; set; }
    }
}

