namespace UIShell.OSGi.Utility
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    public static class FastDirectoryEnumerator
    {
        public static IEnumerable<FileData> EnumerateFiles(string path) => 
            EnumerateFiles(path, "*");

        public static IEnumerable<FileData> EnumerateFiles(string path, string searchPattern) => 
            EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

        public static IEnumerable<FileData> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
            {
                throw new ArgumentOutOfRangeException("searchOption");
            }
            return new FileEnumerable(Path.GetFullPath(path), searchPattern, searchOption);
        }

        public static FileData[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            List<FileData> list = new List<FileData>(EnumerateFiles(path, searchPattern, searchOption));
            FileData[] array = new FileData[list.Count];
            list.CopyTo(array);
            return array;
        }

        private class FileEnumerable : IEnumerable, IEnumerable<FileData>
        {
            private readonly string m_filter;
            private readonly string m_path;
            private readonly SearchOption m_searchOption;

            public FileEnumerable(string path, string filter, SearchOption searchOption)
            {
                this.m_path = path;
                this.m_filter = filter;
                this.m_searchOption = searchOption;
            }

            public IEnumerator<FileData> GetEnumerator() => 
                new FastDirectoryEnumerator.FileEnumerator(this.m_path, this.m_filter, this.m_searchOption);

            IEnumerator IEnumerable.GetEnumerator() => 
                new FastDirectoryEnumerator.FileEnumerator(this.m_path, this.m_filter, this.m_searchOption);
        }

        [SuppressUnmanagedCodeSecurity]
        private class FileEnumerator : IDisposable, IEnumerator, IEnumerator<FileData>
        {
            private Stack<SearchContext> m_contextStack;
            private SearchContext m_currentContext;
            private string m_filter;
            private object m_hndFindFile;
            private string m_path;
            private SearchOption m_searchOption;
            private WIN32_FIND_DATA m_win_find_data = new WIN32_FIND_DATA();

            public FileEnumerator(string path, string filter, SearchOption searchOption)
            {
                this.m_path = path;
                this.m_filter = filter;
                this.m_searchOption = searchOption;
                this.m_currentContext = new SearchContext(path);
                if (this.m_searchOption == SearchOption.AllDirectories)
                {
                    this.m_contextStack = new Stack<SearchContext>();
                }
            }

            public void Dispose()
            {
                if (this.m_hndFindFile != null)
                {
                    this.m_hndFindFile.Dispose();
                }
            }

            [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            private static extern FastDirectoryEnumerator.SafeFindHandle FindFirstFile(string fileName, [In, Out] WIN32_FIND_DATA data);
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            private static extern bool FindNextFile(FastDirectoryEnumerator.SafeFindHandle hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);
            public bool MoveNext()
            {
                bool flag = false;
                if (this.m_currentContext.SubdirectoriesToProcess == null)
                {
                    if (this.m_hndFindFile == null)
                    {
                        new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.m_path).Demand();
                        string fileName = Path.Combine(this.m_path, this.m_filter);
                        this.m_hndFindFile = FindFirstFile(fileName, this.m_win_find_data);
                        flag = !this.m_hndFindFile.IsInvalid;
                    }
                    else
                    {
                        flag = FindNextFile((FastDirectoryEnumerator.SafeFindHandle) this.m_hndFindFile, this.m_win_find_data);
                    }
                }
                if (flag)
                {
                    if ((this.m_win_find_data.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        return this.MoveNext();
                    }
                    return flag;
                }
                if (this.m_searchOption != SearchOption.AllDirectories)
                {
                    return flag;
                }
                if (this.m_currentContext.SubdirectoriesToProcess == null)
                {
                    string[] directories = Directory.GetDirectories(this.m_path);
                    this.m_currentContext.SubdirectoriesToProcess = new Stack<string>(directories);
                }
                if (this.m_currentContext.SubdirectoriesToProcess.Count > 0)
                {
                    string str2 = this.m_currentContext.SubdirectoriesToProcess.Pop();
                    this.m_contextStack.Push(this.m_currentContext);
                    this.m_path = str2;
                    this.m_hndFindFile = null;
                    this.m_currentContext = new SearchContext(this.m_path);
                    return this.MoveNext();
                }
                if (this.m_contextStack.Count <= 0)
                {
                    return flag;
                }
                this.m_currentContext = this.m_contextStack.Pop();
                this.m_path = this.m_currentContext.Path;
                if (this.m_hndFindFile != null)
                {
                    this.m_hndFindFile.Close();
                    this.m_hndFindFile = null;
                }
                return this.MoveNext();
            }

            public void Reset()
            {
                this.m_hndFindFile = null;
            }

            public FileData Current =>
                new FileData(this.m_path, this.m_win_find_data);

            object IEnumerator.Current =>
                new FileData(this.m_path, this.m_win_find_data);

            private class SearchContext
            {
                public readonly string Path;
                public Stack<string> SubdirectoriesToProcess;

                public SearchContext(string path)
                {
                    this.Path = path;
                }
            }
        }

        private sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            internal SafeFindHandle() : base(true)
            {
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll")]
            private static extern bool FindClose(IntPtr handle);
            protected override bool ReleaseHandle() => 
                FindClose(base.handle);
        }
    }
}

