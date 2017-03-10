namespace UIShell.OSGi.Utility
{
    using System;
    using System.IO;

    public static class PathUtility
    {
        public static string GetFullPath(string path)
        {
            AssertUtility.ArgumentHasText(path, "path");
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(BaseDirectory, path);
            }
            return Path.GetFullPath(path);
        }

        public static string BaseDirectory =>
            AppDomain.CurrentDomain.BaseDirectory;
    }
}

