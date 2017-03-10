namespace UIShell.OSGi.Dependency.Resolver
{
    using System;
    using System.Collections.Generic;

    internal interface IHostBundleMetadataNode : IResolvable, IMetadataNode, Interface2, IReferencable
    {
        List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>> AttachAllFragments();
        List<IAssemblyMetadataNode> AttachFragment(IFragmentBundleMetadataNode fragment);
        List<Tuple<IFragmentBundleMetadataNode, List<IAssemblyMetadataNode>>> DetachAllFragments();
        List<IAssemblyMetadataNode> DetachFagment(IFragmentBundleMetadataNode fragment);
        bool IsReferenced();

        List<IFragmentBundleMetadataNode> FragmentNodes { get; }
    }
}

