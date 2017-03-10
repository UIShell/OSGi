namespace UIShell.OSGi.Utility
{
    using System.Threading;

    public static class ReaderWriterLockHelper
    {
        public const int TIMEOUT_INTERNALS = 0x1388;

        public static ReaderLock CreateReaderLock(ReaderWriterLock locker) => 
            new ReaderLock(locker, CurrentThreadId);

        public static WriterLock CreateWriterLock(ReaderWriterLock locker) => 
            new WriterLock(locker, CurrentThreadId);

        private static string CurrentThreadId
        {
            get
            {
                object data = Thread.GetData(Thread.GetNamedDataSlot("EventSlotName"));
                if (data != null)
                {
                    return $"{Thread.CurrentThread.ManagedThreadId}_{Thread.CurrentThread.Name}_{data.ToString()}";
                }
                return $"{Thread.CurrentThread.ManagedThreadId}_{Thread.CurrentThread.Name}";
            }
        }
    }
}

