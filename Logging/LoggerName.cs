namespace UIShell.OSGi.Logging
{
    using System;
    using System.Collections.Generic;

    public sealed class LoggerName : IEquatable<LoggerName>, IComparable<LoggerName>
    {
        private readonly string baseName;
        private readonly string fullName;

        public LoggerName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            fullName = name;
            baseName = ExtractRoot(fullName);
        }

        public LoggerName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            string fullName = type.FullName;
            if (type.IsNested)
            {
                fullName = fullName.Replace('+', '.');
            }
            if (type.IsGenericType)
            {
                int index = fullName.IndexOf('`');
                int length = fullName.IndexOf("[[", index, StringComparison.OrdinalIgnoreCase);
                if (length != -1)
                {
                    fullName = fullName.Substring(0, length);
                }
                fullName = fullName.Replace('`', '_');
            }
            this.fullName = fullName;
            baseName = ExtractRoot(this.fullName);
        }

        public int CompareTo(LoggerName other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return 1;
            }
            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }
            return string.Compare(fullName, other.fullName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) =>
            Equals(obj as LoggerName);

        public bool Equals(LoggerName other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }
            return (object.ReferenceEquals(this, other) || fullName.Equals(other.fullName, StringComparison.OrdinalIgnoreCase));
        }

        private static string ExtractRoot(string fullName)
        {
            string str = fullName;
            int num = fullName.LastIndexOf('.');
            if (num != -1)
            {
                str = fullName.Substring(num + 1);
            }
            return str;
        }

        public override int GetHashCode() =>
            fullName.GetHashCode();

        public static bool operator ==(LoggerName left, LoggerName right) => 
            (left.CompareTo(right) == 0);

        public static bool operator >(LoggerName left, LoggerName right) => 
            (left.CompareTo(right) > 0);

        public static bool operator >=(LoggerName left, LoggerName right) => 
            (left.CompareTo(right) >= 0);

        public static bool operator !=(LoggerName left, LoggerName right) => 
            (left.CompareTo(right) != 0);

        public static bool operator <(LoggerName left, LoggerName right) => 
            (left.CompareTo(right) < 0);

        public static bool operator <=(LoggerName left, LoggerName right) => 
            (left.CompareTo(right) <= 0);

        public override string ToString() =>
            fullName;

        public string BaseName =>
            baseName;

        public string FullName =>
            fullName;

        public IEnumerable<LoggerName> Hierarchy
        {
            get
            {
                yield return this;
                string fullName = this.fullName;
                int length = fullName.LastIndexOf('.');
                while (true)
                {
                    if (length == -1)
                    {
                        yield break;
                    }
                    fullName = fullName.Substring(0, length);
                    length = fullName.LastIndexOf('.');
                    yield return new LoggerName(fullName);
                }
            }
        }

    }
}

