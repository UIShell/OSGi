namespace UIShell.OSGi.Logging.Logs
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UIShell.OSGi.Logging;
    using UIShell.OSGi.Logging.Util;
    using UIShell.OSGi.Utility;

    internal class TextWriterLog : Log, IDisposable
    {
        private TextWriter _writer;
        private volatile bool disposed;
        private readonly TextWriterResponsibility responsibility;

        public TextWriterLog(TextWriter writer) : this(writer, TextWriterResponsibility.DoesNotOwn)
        {
        }

        public TextWriterLog(TextWriter writer, TextWriterResponsibility responsibility)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.responsibility = responsibility;
            this.disposed = false;
            this.Writer = writer;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                try
                {
                    if (disposing && (this.Writer != null))
                    {
                        if (this.responsibility == TextWriterResponsibility.Owns)
                        {
                            this.Writer.Dispose();
                        }
                        this.Writer = null;
                    }
                }
                finally
                {
                    this.disposed = true;
                }
            }
        }

        public override void WriteLine(string message, LogLevel level)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
            try
            {
                this.Writer.WriteLine(message);
                this.Writer.Flush();
            }
            catch (Exception exception)
            {
                if (ExceptionExtensions.IsImportant(exception))
                {
                    throw;
                }
                Trace.TraceWarning(Messages.WriterWriteFailed, new object[] { exception });
            }
        }

        protected virtual TextWriter Writer
        {
            get => 
                this._writer;
            set
            {
                this._writer = value;
            }
        }
    }
}

