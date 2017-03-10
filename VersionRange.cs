namespace UIShell.OSGi
{
    using System;
    using System.ComponentModel;
    using Utility;

    [TypeConverter(typeof(VersionRangeConverter))]
    public class VersionRange : IComparable<VersionRange>, IEquatable<VersionRange>, IComparable
    {
        private const char _includeEqualMaxChar = ']';
        private const char _includeEqualMinChar = '[';
        private string _versionString;

        public bool IsIncludedEqualMaxVersion { get; set; }

        public bool IsIncludedEqualMinVersion { get; set; }

        public Version MaxVersion { get; set; }

        public Version MinVersion { get; set; }

        private VersionRange() : this(null)
        {
        }

        public VersionRange(string version)
        {
            _versionString = version;
            if (version != null)
            {
                version = version.Trim();
            }
            if (string.IsNullOrEmpty(version))
            {
                IsIncludedEqualMaxVersion = true;
                IsIncludedEqualMinVersion = true;
                MinVersion = new Version(0, 0, 0, 0);
                MaxVersion = new Version(0x7fffffff, 0, 0, 0);
            }
            else if (version.IndexOf(',') <= 0)
            {
                IsIncludedEqualMaxVersion = true;
                IsIncludedEqualMinVersion = true;
                MinVersion = new Version(version);
                MaxVersion = VersionUtility.GetMinNotCompatibleVersion(MinVersion);
                IsIncludedEqualMaxVersion = false;
            }
            else
            {
                IsIncludedEqualMinVersion = version.IndexOf('[') == 0;
                IsIncludedEqualMaxVersion = version.IndexOf(']') == (version.Length - 1);
                if (IsIncludedEqualMinVersion)
                {
                    version = version.Substring(1, version.Length - 2);
                }
                string[] strArray = version.Split(new char[] { ',' });
                MinVersion = new Version(strArray[0]);
                MaxVersion = new Version(strArray[1]);
            }
        }

        public VersionRange(string minVersion, string maxVersion)
        {
            MinVersion = new Version(minVersion);
            MaxVersion = new Version(maxVersion);
        }

        public int CompareTo(object obj) =>
            CompareTo(obj as VersionRange);

        public int CompareTo(VersionRange other)
        {
            if (Equals(other))
            {
                return 0;
            }
            return -1;
        }

        public override bool Equals(object obj) => 
            base.Equals(obj as VersionRange);

        public bool Equals(VersionRange other) => 
            ((((other != null) && (other.IsIncludedEqualMaxVersion == IsIncludedEqualMaxVersion)) && ((other.IsIncludedEqualMinVersion == IsIncludedEqualMinVersion) && (other.MaxVersion == MaxVersion))) && (other.MinVersion == MinVersion));

        public override int GetHashCode()
        {
            int num = 0;
            num = 0 | ((MaxVersion.Major & 15) << 0x1c);
            num |= (MaxVersion.Minor & 0xff) << 20;
            num |= (MaxVersion.Build & 0xff) << 12;
            return (num | (MaxVersion.Revision & 0xfff));
        }

        public bool IsIncluded(Version version)
        {
            if (version == null)
            {
                return false;
            }
            int num = version.CompareTo(MinVersion);
            if (IsIncludedEqualMinVersion)
            {
                if (num < 0)
                {
                    return false;
                }
            }
            else if (num <= 0)
            {
                return false;
            }
            num = version.CompareTo(MaxVersion);
            if (IsIncludedEqualMaxVersion)
            {
                if (num > 0)
                {
                    return false;
                }
            }
            else if (num >= 0)
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(VersionRange v1, VersionRange v2)
        {
            if (ReferenceEquals(v1, null))
            {
                return ReferenceEquals(v2, null);
            }
            return v1.Equals(v2);
        }

        public static bool operator >(VersionRange v1, VersionRange v2) => 
            (v2 < v1);

        public static bool operator >=(VersionRange v1, VersionRange v2) => 
            (v2 <= v1);

        public static bool operator !=(VersionRange v1, VersionRange v2) => 
            !(v1 == v2);

        public static bool operator <(VersionRange v1, VersionRange v2) => 
            (v1?.CompareTo(v2) < 0);

        public static bool operator <=(VersionRange v1, VersionRange v2) => 
            (v1?.CompareTo(v2) <= 0);

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_versionString))
            {
                return _versionString;
            }
            if ((MaxVersion == null) && (MinVersion == null))
            {
                return "[0.0.0, Maximum Version)";
            }
            return ("MinVersion:" + MinVersion.ToString() + " MaxVersion:" + MaxVersion.ToString());
        }
    }
}

