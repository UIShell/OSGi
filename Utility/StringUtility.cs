namespace UIShell.OSGi.Utility
{
    using System.Reflection;

    internal class StringUtility
    {
        public static string GetAfter(string source, string splitter)
        {
            int index = source.IndexOf(splitter);
            if (index < 0)
            {
                return string.Empty;
            }
            return source.Substring(index + 1);
        }

        public static bool IsAssemblyFullNameMatch(AssemblyName bundleAssemblyName, AssemblyName toBeLoadedAssemblyName, bool exactMatch)
        {
            if (bundleAssemblyName.FullName != toBeLoadedAssemblyName.FullName)
            {
                if (bundleAssemblyName.Name != toBeLoadedAssemblyName.Name)
                {
                    return false;
                }
                if (exactMatch)
                {
                    if (((bundleAssemblyName.Name != toBeLoadedAssemblyName.Name) || !VersionUtility.Compatible(toBeLoadedAssemblyName.Version, bundleAssemblyName.Version)) || (((bundleAssemblyName.CultureInfo != null) || (toBeLoadedAssemblyName.CultureInfo == null)) && !bundleAssemblyName.CultureInfo.Equals(toBeLoadedAssemblyName.CultureInfo)))
                    {
                        return false;
                    }
                    return ValueEqual(bundleAssemblyName.GetPublicKeyToken(), toBeLoadedAssemblyName.GetPublicKeyToken());
                }
                if (((bundleAssemblyName.CultureInfo != null) && (toBeLoadedAssemblyName.CultureInfo != null)) && bundleAssemblyName.CultureInfo.Equals(toBeLoadedAssemblyName.CultureInfo))
                {
                    return false;
                }
                if (((bundleAssemblyName.Version != null) && (null != toBeLoadedAssemblyName.Version)) && VersionUtility.Compatible(toBeLoadedAssemblyName.Version, bundleAssemblyName.Version))
                {
                    return false;
                }
                if (!ValueEqual(bundleAssemblyName.GetPublicKeyToken(), toBeLoadedAssemblyName.GetPublicKeyToken()))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValueEqual(object token1, object token2)
        {
            if ((token1 != null) && (token2 != null))
            {
                if (token1.Length != token2.Length)
                {
                    return false;
                }
                for (int i = 0; i < token1.Length; i++)
                {
                    if (token1[i] != token2[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

