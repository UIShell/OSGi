namespace UIShell.OSGi.Logging
{
    using System;
    using Utility;

    internal static class LogLevelUtils
    {
        internal static bool IsValid(LogLevel level) => 
            ((level >= LogLevel.Fatal) && (level <= LogLevel.Verbose));

        internal static LogLevel Valid(LogLevel level, string name)
        {
            if (!IsValid(level))
            {
                throw new ArgumentOutOfRangeException(name, level, Messages.LogLevelOutOfRange);
            }
            return level;
        }
    }
}

