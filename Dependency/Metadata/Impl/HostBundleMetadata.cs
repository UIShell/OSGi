namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using System.Collections.Generic;
    using UIShell.OSGi.Dependency.Metadata;

    internal class HostBundleMetadata : BundleMetadata, IMetadata, IBundleMetadata, Interface1
    {
        public HostBundleMetadata()
        {
            this.Fragments = new List<IFragmentBundleMetadata>();
            this.PostDependents = new List<Interface1>();
        }

        public List<IFragmentBundleMetadata> Fragments { get; private set; }

        public List<Interface1> PostDependents { get; private set; }
    }
}

