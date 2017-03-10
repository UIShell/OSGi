namespace UIShell.OSGi.Logging.Logs
{
    using System;
    using UIShell.OSGi.Logging;

    internal class StdErrLog : Log
    {
        public static readonly StdErrLog Instance = new StdErrLog();

        internal StdErrLog()
        {
        }

        public override void WriteLine(string message, LogLevel level)
        {
            Console.Error.WriteLine(message);
        }
    }
}

