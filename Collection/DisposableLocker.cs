namespace UIShell.OSGi.Collection
{
    using System;
    using System.Threading;
    using Utility;

    public class DisposableLocker : IDisposable
    {
        private bool _lockAcquired;
        private int _millisecondsTimeout;
        private object _syncRoot;

        public DisposableLocker(object syncRoot, int millisecondsTimeout)
        {
            AssertUtility.ArgumentNotNull(syncRoot, "syncRoot");
            _syncRoot = syncRoot;
            _millisecondsTimeout = millisecondsTimeout;
            _lockAcquired = Monitor.TryEnter(_syncRoot, _millisecondsTimeout);
            LogWhenAcquireLockFailed();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _lockAcquired)
            {
                Monitor.Exit(_syncRoot);
            }
        }

        ~DisposableLocker()
        {
            Dispose(false);
        }

        private void LogWhenAcquireLockFailed()
        {
            if (!_lockAcquired)
            {
                FileLogUtility.Error(string.Format(Messages.AcquireTheLockTimeout, _millisecondsTimeout));
            }
        }
    }
}

