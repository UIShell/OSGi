namespace UIShell.OSGi.Dependency
{
    using UIShell.OSGi.Dependency.Metadata;

    internal interface IStateDelta
    {
        void RecordBundleAdded(IBundleMetadata bundle);
        void RecordBundleRemovalPending(IBundleMetadata bundle);
        void RecordBundleRemoved(IBundleMetadata bundle);
        void RecordBundleResolved(IBundleMetadata bundle);
        void RecordBundleUnresolved(IBundleMetadata bundle);

        IBundleDelta[] Changes { get; }

        IState State { get; set; }
    }
}

