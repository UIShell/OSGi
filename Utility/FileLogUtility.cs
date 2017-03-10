namespace UIShell.OSGi.Utility
{
    using System;
    using System.IO;
    using UIShell.OSGi.Logging;
    using UIShell.OSGi.Logging.Logs;

    public static class FileLogUtility
    {
        private static Logger _logger;
        internal static bool CREATE_NEW_ON_FILE_MAX_SIZE = false;
        internal static int DEFAULT_MAX_SIZE_MB = 10;
        internal static int MAX_FILE_SIZE_BYTE = (DEFAULT_MAX_SIZE_MB * 0x100000);
        private const int MB_TO_BYTE = 0x100000;

        static FileLogUtility()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(FileLogUtility.CurrentDomainUnhandledException);
            Log instance = null;
            try
            {
                instance = new FileLog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"));
            }
            catch
            {
                instance = NullLog.Instance;
            }
            _logger = new Logger("UIShell.OSGi.Logger", instance);
            SetLogLevel(UIShell.OSGi.Logging.LogLevel.Debug);
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exceptionObject = e.ExceptionObject as Exception;
            if (exceptionObject != null)
            {
                Fatal(string.Format(Messages.UnhandledExceptionMsg, sender));
                Fatal(exceptionObject);
            }
            else
            {
                Fatal(string.Format(Messages.UnhandledExceptionMsg, sender));
            }
        }

        public static void Debug(Exception ex)
        {
            _logger.Debug(ex);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Debug(MessageGenerator message)
        {
            _logger.Debug(message);
        }

        public static void Error(Exception ex)
        {
            _logger.Error(ex);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Error(MessageGenerator message)
        {
            _logger.Error(message);
        }

        public static void Fatal(Exception ex)
        {
            _logger.Fatal(ex);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Fatal(MessageGenerator message)
        {
            _logger.Fatal(message);
        }

        public static void Inform(Exception ex)
        {
            _logger.Inform(ex);
        }

        public static void Inform(string message)
        {
            _logger.Inform(message);
        }

        public static void Inform(MessageGenerator message)
        {
            _logger.Inform(message);
        }

        public static void SetCreateNewFileOnMaxSize(bool createNew)
        {
            CREATE_NEW_ON_FILE_MAX_SIZE = createNew;
        }

        public static void SetLogLevel(UIShell.OSGi.Logging.LogLevel level)
        {
            _logger.Level = level;
        }

        public static void SetMaxFileSizeByMB(int sizeMB)
        {
            if (sizeMB <= 0)
            {
                sizeMB = DEFAULT_MAX_SIZE_MB;
            }
            MAX_FILE_SIZE_BYTE = sizeMB * 0x100000;
        }

        public static void Verbose(Exception ex)
        {
            _logger.Verbose(ex);
        }

        public static void Verbose(string message)
        {
            _logger.Verbose(message);
        }

        public static void Verbose(MessageGenerator message)
        {
            _logger.Verbose(message);
        }

        public static void Warn(Exception ex)
        {
            _logger.Warn(ex);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Warn(MessageGenerator message)
        {
            _logger.Warn(message);
        }

        public static UIShell.OSGi.Logging.LogLevel LogLevel =>
            _logger.Level;
    }
}

