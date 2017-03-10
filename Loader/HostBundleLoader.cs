namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;
    using Core.Adaptor;
    using Core.Bundle;
    using Dependency.Metadata;

    internal class HostBundleLoader : BundleLoader
    {
        public HostBundleLoader(HostBundle bundle)
            : base(bundle)
        {
            Framework = bundle.Framework;
            FrameworkAdaptor = bundle.Framework.FrameworkAdaptor;
        }

        public override Type LoadClass(string className)
        {
            Predicate<IBundleMetadata> match = null;
            Predicate<IFragmentBundleMetadata> predicate2 = null;
            var type = base.LoadClass(className);
            if (type == null)
            {
                if (match == null)
                {
                    match = a => a.BundleID == Bundle.BundleID;
                }
                Interface1 interface2 = FrameworkAdaptor.State.Bundles.Find(match) as Interface1;
                if (predicate2 == null)
                {
                    predicate2 = a => Framework.Bundles.Exists(bundle => (bundle.BundleID == a.BundleID) && ((type = (bundle as FragmentBundle).LoadClassForHost(className)) != null));
                }
                if (interface2.Fragments.Exists(predicate2))
                {
                    if (type != null)
                    {
                        CacheLoadedType(className, type);
                    }
                    return type;
                }
            }
            return type;
        }

        public override object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode)
        {
            Predicate<IBundleMetadata> match = null;
            Predicate<IFragmentBundleMetadata> predicate2 = null;
            object result = base.LoadResource(resourceName, resourceLoadMode);
            if ((result == null) && ((resourceLoadMode == ResourceLoadMode.LocalAndFragment) || (resourceLoadMode == ResourceLoadMode.ClassSpace)))
            {
                if (match == null)
                {
                    match = a => a.BundleID == Bundle.BundleID;
                }
                Interface1 interface2 = FrameworkAdaptor.State.Bundles.Find(match) as Interface1;
                if (predicate2 == null)
                {
                    predicate2 = delegate (IFragmentBundleMetadata a) {
                        IBundle bundle = Framework.Bundles.Find(bundle => bundle.BundleID == a.BundleID);
                        if (bundle == null)
                        {
                            return false;
                        }
                        return (result = bundle.LoadResource(resourceName, resourceLoadMode)) != null;
                    };
                }
                if (interface2.Fragments.Exists(predicate2))
                {
                    return result;
                }
            }
            return result;
        }

        private IFrameworkAdaptor FrameworkAdaptor { get; set; }
    }
}

