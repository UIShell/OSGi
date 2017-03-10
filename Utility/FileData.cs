namespace UIShell.OSGi.Utility
{
    using System;
    using System.IO;

    [Serializable]
    public class FileData
    {
        public readonly FileAttributes Attributes;
        public readonly DateTime CreationTimeUtc;
        public readonly DateTime LastAccessTimeUtc;
        public readonly DateTime LastWriteTimeUtc;
        public readonly string Name;
        public readonly string Path;
        public readonly long Size;

        internal FileData(string dir, WIN32_FIND_DATA findData)
        {
            this.Attributes = findData.dwFileAttributes;
            this.CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime, findData.ftCreationTime_dwLowDateTime);
            this.LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime, findData.ftLastAccessTime_dwLowDateTime);
            this.LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime, findData.ftLastWriteTime_dwLowDateTime);
            this.Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);
            this.Name = findData.cFileName;
            this.Path = System.IO.Path.Combine(dir, findData.cFileName);
        }

        private static long CombineHighLowInts(uint high, uint low) => 
            ((long) ((high << 0x20) | low));

        private static DateTime ConvertDateTime(uint high, uint low) => 
            DateTime.FromFileTimeUtc(CombineHighLowInts(high, low));

        public override string ToString() => 
            this.Name;

        public DateTime CreationTime =>
            this.CreationTimeUtc.ToLocalTime();

        public DateTime LastAccesTime =>
            this.LastAccessTimeUtc.ToLocalTime();

        public DateTime LastWriteTime =>
            this.LastWriteTimeUtc.ToLocalTime();
    }
}

