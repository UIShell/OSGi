namespace UIShell.OSGi.Logging.Logs
{
    using System;
    using System.IO;
    using Utility;

    internal class FileLog : TextWriterLog
    {
        private FileInfo _fileInfo;
        private FileStream _stream;
        private object _syncRoot;

        public FileLog(FileInfo info) : this(info, true)
        {
        }

        public FileLog(string filePath) : this(filePath, true)
        {
        }

        public FileLog(FileInfo info, bool append) : base(CreateWriter(info, append), TextWriterResponsibility.Owns)
        {
            this._syncRoot = new object();
            this._fileInfo = info;
        }

        public FileLog(string filePath, bool append) : this(new FileInfo(filePath), append)
        {
        }

        private static TextWriter CreateWriter(object info, bool append)
        {
            FileStream stream = null;
            if (append)
            {
                stream = info.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            }
            else if (info.Exists)
            {
                stream = info.Open(FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
            }
            else
            {
                stream = info.Open(FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
            }
            return new StreamWriter(stream);
        }

        protected override TextWriter Writer
        {
            get
            {
                if ((this._stream != null) && (this._stream.Length > FileLogUtility.MAX_FILE_SIZE_BYTE))
                {
                    lock (this._syncRoot)
                    {
                        if ((this._stream != null) && (this._stream.Length > FileLogUtility.MAX_FILE_SIZE_BYTE))
                        {
                            this._stream = null;
                            base.Writer.Dispose();
                            base.Writer = null;
                            if (FileLogUtility.CREATE_NEW_ON_FILE_MAX_SIZE)
                            {
                                File.Delete(this._fileInfo.FullName);
                            }
                            else
                            {
                                File.Copy(this._fileInfo.FullName, Path.Combine(this._fileInfo.DirectoryName, $"log{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}.txt"));
                            }
                            base.Writer = CreateWriter(this._fileInfo, false);
                            this._stream = (base.Writer as StreamWriter).BaseStream as FileStream;
                        }
                    }
                }
                return base.Writer;
            }
            set
            {
                base.Writer = value;
                this._stream = (base.Writer as StreamWriter).BaseStream as FileStream;
            }
        }
    }
}

