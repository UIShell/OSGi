namespace UIShell.OSGi.Logging
{
    using System;

    public interface ILogger
    {
        void Debug(Exception ex);
        void Debug(string message);
        void Debug(MessageGenerator message);
        void Error(Exception ex);
        void Error(string message);
        void Error(MessageGenerator message);
        void Fatal(Exception ex);
        void Fatal(string message);
        void Fatal(MessageGenerator message);
        void Inform(Exception ex);
        void Inform(string message);
        void Inform(MessageGenerator message);
        void Verbose(Exception ex);
        void Verbose(string message);
        void Verbose(MessageGenerator message);
        void Warn(Exception ex);
        void Warn(string message);
        void Warn(MessageGenerator message);
    }
}

