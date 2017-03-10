namespace UIShell.OSGi.Logging.Logs
{
    using UIShell.OSGi.Logging;

    internal class NullLog : Log
    {
        public static readonly NullLog Instance = new NullLog();

        private NullLog()
        {
        }

        public override void WriteLine(string message, LogLevel level)
        {
        }

        public override bool IsLogging
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
    }
}

