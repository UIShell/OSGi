namespace UIShell.OSGi.Utility
{
    using System;

    public static class VersionUtility
    {
        public static bool Compatible(Version oldVersion, Version newVersion)
        {
            if ((oldVersion == null) || (newVersion == null))
            {
                return false;
            }
            return ((newVersion.Major == oldVersion.Major) && (newVersion.CompareTo(oldVersion) >= 0));
        }

        public static Version GetMinNotCompatibleVersion(Version version)
        {
            if (version == null)
            {
                return null;
            }
            return new Version(version.Major + 1, 0, 0, 0);
        }
    }
}

