namespace UIShell.OSGi.Loader
{
    using OSGi;
    using Core.Bundle;

    internal class FragementBundleLoader : BundleLoader
    {
        public FragementBundleLoader(IBundle bundle)
            : base(bundle)
        {
            var fragmentBundle = bundle as FragmentBundle;
            Framework = fragmentBundle.Framework;
        }
    }
}

