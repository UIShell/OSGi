namespace UIShell.OSGi.Logging.Logs
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Logging;
    using Utility;

    internal class LockingLog : Log, IDisposable
    {
        private volatile bool disposed;
        private readonly TimeSpan maxWaitTimeForLock;
        private Log wrapped;
        private readonly object wrappedLock;

        public LockingLog(Log toWrap) : this(toWrap, TimeSpan.FromSeconds(2.0))
        {
        }

        public LockingLog(Log toWrap, TimeSpan maxWaitTime)
        {
            if (toWrap == null)
            {
                throw new ArgumentNullException("toWrap");
            }
            this.maxWaitTimeForLock = maxWaitTime;
            this.wrappedLock = new object();
            this.disposed = false;
            this.wrapped = toWrap;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            ActionDelegate delegate2 = null;
            if (!this.disposed && disposing)
            {
                if (delegate2 == null)
                {
                    delegate2 = delegate {
                        try
                        {
                            IDisposable wrapped = this.wrapped as IDisposable;
                            if (wrapped != null)
                            {
                                wrapped.Dispose();
                            }
                            this.wrapped = null;
                        }
                        finally
                        {
                            this.disposed = true;
                        }
                    };
                }
                ActionDelegate action = delegate2;
                this.ExecuteInsideLock(action);
            }
        }

        private void ExecuteInsideLock(ActionDelegate action)
        {
            if (Monitor.TryEnter(this.wrappedLock, this.maxWaitTimeForLock))
            {
                try
                {
                    action();
                    return;
                }
                finally
                {
                    Monitor.Exit(this.wrappedLock);
                }
            }
            Trace.TraceWarning(Messages.LockingLogTimeout);
        }

        private string GetObjectName() => 
            string.Concat(new object[] { base.GetType().FullName, '[', this.wrapped.GetType().FullName, ']' });

        public override string ToString() => 
            this.GetObjectName();

        public override void WriteLine(string message, LogLevel level)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetObjectName());
            }
            this.ExecuteInsideLock(() => this.wrapped.WriteLine(message, level));
        }
    }
}

