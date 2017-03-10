namespace UIShell.OSGi.Logging.Util
{
    using System;
    using System.Threading;

    internal static class ExceptionExtensions
    {
        internal static bool IsImportant(Exception ex)
        {
            if (ex == null)
            {
                return false;
            }
            return (((ex is StackOverflowException) || (ex is OutOfMemoryException)) || (ex is ThreadAbortException));
        }
    }
}

