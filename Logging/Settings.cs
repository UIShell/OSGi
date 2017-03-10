namespace UIShell.OSGi.Logging
{
    using System;
    using System.Collections.Generic;
    using Logs;

    public class Settings
    {
        private LogLevel defaultLevel = LogLevel.Inform;
        private Log defaultLog = StdErrLog.Instance;
        private readonly Dictionary<LoggerName, ExceptionFormatter> nameToExceptionFormatterMapping = new Dictionary<LoggerName, ExceptionFormatter>();
        private readonly Dictionary<LoggerName, LogLevel> nameToLevelMapping = new Dictionary<LoggerName, LogLevel>();
        private readonly Dictionary<LoggerName, Log> nameToLogMapping = new Dictionary<LoggerName, Log>();
        private readonly Dictionary<LoggerName, MessageFormatter> nameToMessageFormatterMapping = new Dictionary<LoggerName, MessageFormatter>();

        public ExceptionFormatter FindExceptionFormatterFor<T>() =>
            FindExceptionFormatterFor(new LoggerName(typeof(T)));

        public ExceptionFormatter FindExceptionFormatterFor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return FindExceptionFormatterFor(new LoggerName(name));
        }

        public ExceptionFormatter FindExceptionFormatterFor(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return FindExceptionFormatterFor(new LoggerName(type));
        }

        internal ExceptionFormatter FindExceptionFormatterFor(LoggerName name) => 
            FindValueFor<ExceptionFormatter>(name, nameToExceptionFormatterMapping, DefaultFormatters.Exception);

        public Log FindLogFor<T>() =>
            FindLogFor(new LoggerName(typeof(T)));

        public Log FindLogFor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return FindLogFor(new LoggerName(name));
        }

        public Log FindLogFor(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return FindLogFor(new LoggerName(type));
        }

        internal Log FindLogFor(LoggerName name) => 
            FindValueFor<Log>(name, nameToLogMapping, defaultLog);

        public LogLevel FindLogLevelFor<T>() =>
            FindLogLevelFor(new LoggerName(typeof(T)));

        public LogLevel FindLogLevelFor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return FindLogLevelFor(new LoggerName(name));
        }

        public LogLevel FindLogLevelFor(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return FindLogLevelFor(new LoggerName(type));
        }

        internal LogLevel FindLogLevelFor(LoggerName name) => 
            FindValueFor<LogLevel>(name, nameToLevelMapping, defaultLevel);

        public MessageFormatter FindMessageFormatterFor<T>() =>
            FindMessageFormatterFor(new LoggerName(typeof(T)));

        public MessageFormatter FindMessageFormatterFor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return FindMessageFormatterFor(new LoggerName(name));
        }

        public MessageFormatter FindMessageFormatterFor(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return FindMessageFormatterFor(new LoggerName(type));
        }

        internal MessageFormatter FindMessageFormatterFor(LoggerName name) => 
            FindValueFor<MessageFormatter>(name, nameToMessageFormatterMapping, DefaultFormatters.Message);

        private static TValue FindValueFor<TValue>(LoggerName name, Dictionary<LoggerName, TValue> valuesDictionary, object defaultValue)
        {
            TValue local = (TValue) defaultValue;
            if (valuesDictionary.Count > 0)
            {
                using (IEnumerator<LoggerName> enumerator = name.Hierarchy.GetEnumerator())
                {
                    TValue local2;
                    while (enumerator.MoveNext())
                    {
                        LoggerName current = enumerator.Current;
                        if (valuesDictionary.TryGetValue(current, out local2))
                        {
                            goto Label_0036;
                        }
                    }
                    return local;
                Label_0036:
                    local = local2;
                }
            }
            return local;
        }

        public void RegisterExceptionFormatterFor<T>(ExceptionFormatter formatter)
        {
            RegisterExceptionFormatterFor(new LoggerName(typeof(T)), formatter);
        }

        public void RegisterExceptionFormatterFor(string name, ExceptionFormatter formatter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            RegisterExceptionFormatterFor(new LoggerName(name), formatter);
        }

        public void RegisterExceptionFormatterFor(Type type, ExceptionFormatter formatter)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RegisterExceptionFormatterFor(new LoggerName(type), formatter);
        }

        private void RegisterExceptionFormatterFor(LoggerName name, ExceptionFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            RegisterValueFor<ExceptionFormatter>(name, nameToExceptionFormatterMapping, formatter);
        }

        public void RegisterLogFor<T>(Log log)
        {
            RegisterLogFor(new LoggerName(typeof(T)), log);
        }

        public void RegisterLogFor(string name, Log log)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            RegisterLogFor(new LoggerName(name), log);
        }

        public void RegisterLogFor(Type type, Log log)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RegisterLogFor(new LoggerName(type), log);
        }

        private void RegisterLogFor(LoggerName name, Log log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            RegisterValueFor<Log>(name, nameToLogMapping, log);
        }

        public void RegisterLogLevelFor<T>(LogLevel logLevel)
        {
            RegisterLogLevelFor(new LoggerName(typeof(T)), logLevel);
        }

        public void RegisterLogLevelFor(string name, LogLevel logLevel)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            RegisterLogLevelFor(new LoggerName(name), logLevel);
        }

        public void RegisterLogLevelFor(Type type, LogLevel logLevel)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RegisterLogLevelFor(new LoggerName(type), logLevel);
        }

        private void RegisterLogLevelFor(LoggerName name, LogLevel level)
        {
            LogLevelUtils.Valid(level, "level");
            RegisterValueFor<LogLevel>(name, nameToLevelMapping, level);
        }

        public void RegisterMessageFormatterFor<T>(MessageFormatter formatter)
        {
            RegisterMessageFormatterFor(new LoggerName(typeof(T)), formatter);
        }

        public void RegisterMessageFormatterFor(string name, MessageFormatter formatter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            RegisterMessageFormatterFor(new LoggerName(name), formatter);
        }

        public void RegisterMessageFormatterFor(Type type, MessageFormatter formatter)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RegisterMessageFormatterFor(new LoggerName(type), formatter);
        }

        private void RegisterMessageFormatterFor(LoggerName name, MessageFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            RegisterValueFor<MessageFormatter>(name, nameToMessageFormatterMapping, formatter);
        }

        private static void RegisterValueFor<TValue>(object name, Dictionary<LoggerName, TValue> valuesDictionary, object value)
        {
            valuesDictionary[(LoggerName) name] = (TValue) value;
        }

        public static Settings Default
        {
            get
            {
              return DefaultSettingsHolder.Default;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                DefaultSettingsHolder.Default = value;
            }
        }

        public Log DefaultLog
        {
            get
            {
                return defaultLog;
            }
            set
            {
                if (value == null)
                {
                    value = StdErrLog.Instance;
                }
                defaultLog = value;
            }
        }

        public LogLevel DefaultLogLevel
        {
            get
            {
                return defaultLevel;
            }
            set
            {
                defaultLevel = LogLevelUtils.Valid(value, "value");
            }
        }

        private static class DefaultSettingsHolder
        {
            private static Settings instance = new Settings();

            internal static Settings Default
            {
                get
                {
                    return instance;
                }
                set
                {
                    instance = value;
                }
            }
        }
    }
}

