namespace UIShell.OSGi.Logging
{
    using System;

    public delegate string ExceptionFormatter(Exception ex, LoggerName name, LogLevel level);
}

