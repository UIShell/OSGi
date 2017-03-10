namespace UIShell.OSGi.Utility
{
    using System;
    using System.Threading;

    public class WriterLock : IDisposable
    {
        private ReaderWriterLock _locker;
        private string _owner;

        public WriterLock(ReaderWriterLock locker, string owner)
        {
            _locker = locker;
            _owner = owner;
            if (!_locker.IsWriterLockHeld)
            {
                try
                {
                    if (_locker.IsReaderLockHeld)
                    {
                        _locker.UpgradeToWriterLock(0x1388);
                    }
                    else
                    {
                        _locker.AcquireWriterLock(0x1388);
                    }
                    FileLogUtility.Verbose($"{_owner} accquires writer lock successfully.");
                }
                catch (Exception exception)
                {
                    FileLogUtility.Verbose($"{_owner} accquires writer lock failed.");
                    FileLogUtility.Warn(Messages.AccquireWriterLockTimeOut);
                    FileLogUtility.Warn(exception);
                }
            }
        }

        public void Dispose()
        {
            if (_locker.IsWriterLockHeld)
            {
                _locker.ReleaseWriterLock();
                FileLogUtility.Verbose($"{_owner} releases writer lock successfully.");
            }
            _locker = null;
            GC.SuppressFinalize(this);
        }
    }
}

