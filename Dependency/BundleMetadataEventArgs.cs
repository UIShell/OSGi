namespace UIShell.OSGi.Dependency
{
    using System;
    using UIShell.OSGi.Dependency.Metadata;

    internal class BundleMetadataEventArgs : EventArgs
    {
        private IBundleMetadata _bundle;

        public BundleMetadataEventArgs(IBundleMetadata bundle)
        {
            this._bundle = bundle;
        }

        public IBundleMetadata Metadata =>
            this._bundle;
    }
}

