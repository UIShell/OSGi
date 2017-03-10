namespace UIShell.OSGi.Logging
{
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using Util;
    using Utility;

    internal static class DefaultFormatters
    {
        private static ExceptionFormatter defaultExceptionFormatter;
        private static MessageFormatter defaultMessageFormatter;
        private const string LOCALE_ZH_CN = "zh-CN";

        static DefaultFormatters()
        {
            Reset();
        }

        private static string _DefaultExceptionFormatter(object ex, object name, LogLevel level)
        {
            string str = string.Empty;
            if (ex != null)
            {
                str = ex.ToString();
            }
            CultureInfo provider = new CultureInfo("zh-CN");
            string str5 = Clock.Now.ToString("R", provider);
            string str2 = Thread.CurrentThread.Name ?? Messages.NoThread;
            string str3 = name.ToString();
            string str4 = level.ToString();
            float num = (((str5.Length + str2.Length) + str3.Length) + str4.Length) + str.Length;
            num *= 1.05f;
            return new StringBuilder((int) num).Append(str5).Append("|***|").Append(str2).Append('|').Append(str3).Append("|   ").Append('|').Append(str4).Append(str).ToString();
        }

        private static string _DefaultMessageFormatter(string userMessage, object name, LogLevel level)
        {
            if (userMessage == null)
            {
                userMessage = string.Empty;
            }
            CultureInfo provider = new CultureInfo("zh-CN");
            string str4 = Clock.Now.ToString("R", provider);
            string str = Thread.CurrentThread.Name ?? Messages.NoThread;
            string str2 = name.ToString();
            string str3 = level.ToString();
            float num = (((str4.Length + str.Length) + str2.Length) + str3.Length) + userMessage.Length;
            num *= 1.05f;
            return new StringBuilder((int) num).Append(str4).Append("|   |").Append(str).Append('|').Append(str2).Append('|').Append(str3).Append("|   ").Append(userMessage).ToString();
        }

        public static void Reset()
        {
            defaultMessageFormatter = new MessageFormatter(_DefaultMessageFormatter);
            defaultExceptionFormatter = new ExceptionFormatter(_DefaultExceptionFormatter);
        }

        public static ExceptionFormatter Exception
        {
            get
            {
                return defaultExceptionFormatter;
            }
            set
            {
                if (value == null)
                {
                    value = new ExceptionFormatter(_DefaultExceptionFormatter);
                }
                defaultExceptionFormatter = value;
            }
        }

        public static MessageFormatter Message
        {
            get
            {
                return defaultMessageFormatter;
            }
            set
            {
                if (value == null)
                {
                    value = new MessageFormatter(_DefaultMessageFormatter);
                }
                defaultMessageFormatter = value;
            }
        }
    }
}

