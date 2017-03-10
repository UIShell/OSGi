namespace UIShell.OSGi
{
    using System;
    using System.IO;
    using Utility;

    public static class BundleUrlHelper
    {
        public static string Content(string symbolicName, string url)
        {
            var bundleBySymbolicName = BundleRuntime.Instance.Framework.Bundles.GetBundleBySymbolicName(symbolicName);
            if (bundleBySymbolicName == null)
            {
                throw new Exception(string.Format(Messages.BundleNotExist, symbolicName));
            }
            return Content(bundleBySymbolicName, url);
        }

        public static string Content(IBundle bundle, string url)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            AssertUtility.ArgumentHasText(url, "url");
            while (url.StartsWith("~") || (url.StartsWith("/") || url.StartsWith(@"\")))
            {
                url = url.Remove(0, 1);
            }
            return Path.Combine(bundle.Location, url);
        }
    }
}

