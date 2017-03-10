namespace UIShell.OSGi.Logging
{
    using System;

    internal class Logger : ILogger
    {
        private Log destinationLog;
        private ExceptionFormatter exceptionFormatter;
        private LogLevel level;
        private MessageFormatter messageFormatter;
        private LoggerName name;
        private Settings settings;

        public Logger(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this.name = new LoggerName(name);
            settings = Settings.Default;
            destinationLog = settings.FindLogFor(this.name);
            CommonInit();
        }

        public Logger(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            name = new LoggerName(type);
            settings = Settings.Default;
            destinationLog = settings.FindLogFor(name);
            CommonInit();
        }

        public Logger(string name, Log log)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            this.name = new LoggerName(name);
            settings = Settings.Default;
            destinationLog = log;
            CommonInit();
        }

        public Logger(string name, Settings settings)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            this.name = new LoggerName(name);
            this.settings = settings;
            destinationLog = this.settings.FindLogFor(this.name);
            CommonInit();
        }

        public Logger(Type type, Log log)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            name = new LoggerName(type);
            settings = Settings.Default;
            destinationLog = log;
            CommonInit();
        }

        public Logger(Type type, Settings settings)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            name = new LoggerName(type);
            this.settings = settings;
            destinationLog = this.settings.FindLogFor(name);
            CommonInit();
        }

        public Logger(string name, Log log, Settings settings)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            this.name = new LoggerName(name);
            this.settings = settings;
            destinationLog = log;
            CommonInit();
        }

        public Logger(Type type, Log log, Settings settings)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            name = new LoggerName(type);
            this.settings = settings;
            destinationLog = log;
            CommonInit();
        }

        private void CommonInit()
        {
            level = settings.FindLogLevelFor(name);
            messageFormatter = settings.FindMessageFormatterFor(name);
            exceptionFormatter = settings.FindExceptionFormatterFor(name);
        }

        public void Debug(Exception ex)
        {
            Log(ex, LogLevel.Debug);
        }

        public void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public void Debug(MessageGenerator message)
        {
            Log(message, LogLevel.Debug);
        }

        public void Error(Exception ex)
        {
            Log(ex, LogLevel.Error);
        }

        public void Error(string message)
        {
            Log(message, LogLevel.Error);
        }

        public void Error(MessageGenerator message)
        {
            Log(message, LogLevel.Error);
        }

        public void Fatal(Exception ex)
        {
            Log(ex, LogLevel.Fatal);
        }

        public void Fatal(string message)
        {
            Log(message, LogLevel.Fatal);
        }

        public void Fatal(MessageGenerator message)
        {
            Log(message, LogLevel.Fatal);
        }

        private string Format(Exception ex, LogLevel exceptionLevel) =>
            exceptionFormatter(ex, name, exceptionLevel);

        private string Format(string message, LogLevel messageLevel) =>
            messageFormatter(message, name, messageLevel);

        public void Inform(Exception ex)
        {
            Log(ex, LogLevel.Inform);
        }

        public void Inform(string message)
        {
            Log(message, LogLevel.Inform);
        }

        public void Inform(MessageGenerator message)
        {
            Log(message, LogLevel.Inform);
        }

        public void Log(Exception ex, LogLevel exceptionLevel)
        {
            if (WillLogAt(exceptionLevel))
            {
                destinationLog.WriteLine(Format(ex, exceptionLevel), exceptionLevel);
            }
        }

        public void Log(string message, LogLevel messageLevel)
        {
            if (WillLogAt(messageLevel))
            {
                destinationLog.WriteLine(Format(message, messageLevel), messageLevel);
            }
        }

        public void Log(MessageGenerator message, LogLevel messageLevel)
        {
            if (WillLogAt(messageLevel))
            {
                string str = string.Empty;
                if (message != null)
                {
                    str = message();
                }
                destinationLog.WriteLine(Format(str, messageLevel), messageLevel);
            }
        }

        public void Verbose(Exception ex)
        {
            Log(ex, LogLevel.Verbose);
        }

        public void Verbose(string message)
        {
            Log(message, LogLevel.Verbose);
        }

        public void Verbose(MessageGenerator message)
        {
            Log(message, LogLevel.Verbose);
        }

        public void Warn(Exception ex)
        {
            Log(ex, LogLevel.Warn);
        }

        public void Warn(string message)
        {
            Log(message, LogLevel.Warn);
        }

        public void Warn(MessageGenerator message)
        {
            Log(message, LogLevel.Warn);
        }

        public bool WillLogAt(LogLevel queryLevel) => 
            ((destinationLog.IsLogging && LogLevelUtils.IsValid(queryLevel)) && (level >= queryLevel));

        public Log DestinationLog
        {
            get { return destinationLog; }
            set
            {
                if (value == null)
                {
                    value = settings.FindLogFor(name);
                }
                destinationLog = value;
            }
        }

        public ExceptionFormatter ExceptionFormatter
        {
            get { return exceptionFormatter; }
            set
            {
                if (value == null)
                {
                    value = settings.FindExceptionFormatterFor(name);
                }
                exceptionFormatter = value;
            }
        }

        public LogLevel Level
        {
            get { return level; }
            set
            {
                level = LogLevelUtils.Valid(value, "value");
            }
        }

        public MessageFormatter MessageFormatter
        {
            get { return messageFormatter; }
            set
            {
                if (value == null)
                {
                    value = settings.FindMessageFormatterFor(name);
                }
                messageFormatter = value;
            }
        }
    }
}

