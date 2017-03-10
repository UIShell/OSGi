namespace UIShell.OSGi.Dependency.Metadata
{
    using System.Collections.Generic;

    internal interface Interface1 : IMetadata, IBundleMetadata
    {
        List<IFragmentBundleMetadata> Fragments { get; }

        List<Interface1> PostDependents { get; }
    }
}

