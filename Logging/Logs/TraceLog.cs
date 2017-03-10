namespace UIShell.OSGi.Logging.Logs
{
    using System.Diagnostics;
    using Logging;

    internal class TraceLog : Log
    {
        public static readonly TraceLog Instance = new TraceLog();

        private TraceLog()
        {
        }

        public override void WriteLine(string message, LogLevel level)
        {
            Trace.WriteLine(message);
        }
    }
}

