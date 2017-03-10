namespace UIShell.OSGi.Utility
{
    using System;
    using System.Threading;

    public class ReaderLock : IDisposable
    {
        private ReaderWriterLock _locker;
        private string _owner;

        public ReaderLock(ReaderWriterLock locker, string owner)
        {
            _locker = locker;
            _owner = owner;
            if (!_locker.IsReaderLockHeld && !_locker.IsWriterLockHeld)
            {
                try
                {
                    _locker.AcquireReaderLock(0x1388);
                    FileLogUtility.Verbose($"{_owner} accquires reader lock successfully.");
                }
                catch (Exception exception)
                {
                    FileLogUtility.Verbose($"{_owner} accquires reader lock failed.");
                    FileLogUtility.Warn(Messages.AccquireReaderLockTimeOut);
                    FileLogUtility.Warn(exception);
                }
            }
        }

        public void Dispose()
        {
            if (_locker.IsReaderLockHeld)
            {
                _locker.ReleaseReaderLock();
                FileLogUtility.Verbose($"{_owner} releases reader lock.");
            }
            _locker = null;
            GC.SuppressFinalize(this);
        }
    }
}

