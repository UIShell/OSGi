namespace UIShell.OSGi.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Reflection;
    using System.Security;
    using System.Security.Principal;
    using System.Threading;

    public abstract class ExceptionFormatter
    {
        private NameValueCollection additionalInfo;
        private static readonly ArrayList IgnoredProperties = new ArrayList(new string[] { "Source", "Message", "HelpLink", "InnerException", "StackTrace" });

        public NameValueCollection AdditionalInfo
        {
            get
            {
                if (additionalInfo == null)
                {
                    additionalInfo = new NameValueCollection();
                    additionalInfo.Add("MachineName", GetMachineName());
                    additionalInfo.Add("TimeStamp", DateTime.UtcNow.ToString(CultureInfo.CurrentCulture));
                    additionalInfo.Add("FullName", Assembly.GetExecutingAssembly().FullName);
                    additionalInfo.Add("AppDomainName", AppDomain.CurrentDomain.FriendlyName);
                    additionalInfo.Add("ThreadIdentity", Thread.CurrentPrincipal.Identity.Name);
                    additionalInfo.Add("WindowsIdentity", GetWindowsIdentity());
                }
                return additionalInfo;
            }
        }

        public Exception Exception { get; private set; }

        protected ExceptionFormatter(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            Exception = exception;
        }

        public virtual void Format()
        {
            WriteDescription();
            WriteDateTime(DateTime.UtcNow);
            WriteException(Exception, null);
        }

        private static string GetMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (SecurityException)
            {
                return Messages.PermissionDenied;
            }
        }

        private static string GetWindowsIdentity()
        {
            try
            {
                return WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
                return Messages.PermissionDenied;
            }
        }

        protected abstract void WriteAdditionalInfo(NameValueCollection additionalInformation);
        protected abstract void WriteDateTime(DateTime utcNow);
        protected abstract void WriteDescription();
        protected virtual void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (exceptionToFormat == null)
            {
                throw new ArgumentNullException("exceptionToFormat");
            }
            WriteExceptionType(exceptionToFormat.GetType());
            WriteMessage(exceptionToFormat.Message);
            WriteSource(exceptionToFormat.Source);
            WriteHelpLink(exceptionToFormat.HelpLink);
            WriteReflectionInfo(exceptionToFormat);
            WriteStackTrace(exceptionToFormat.StackTrace);
            if (outerException == null)
            {
                WriteAdditionalInfo(AdditionalInfo);
            }
            var innerException = exceptionToFormat.InnerException;
            if (innerException != null)
            {
                WriteException(innerException, exceptionToFormat);
            }
        }

        protected abstract void WriteExceptionType(Type exceptionType);
        protected abstract void WriteFieldInfo(FieldInfo fieldInfo, object value);
        protected abstract void WriteHelpLink(string helpLink);
        protected abstract void WriteMessage(string message);
        protected abstract void WritePropertyInfo(PropertyInfo propertyInfo, object value);
        protected void WriteReflectionInfo(Exception exceptionToFormat)
        {
            object fieldAccessFailed;
            if (exceptionToFormat == null)
            {
                throw new ArgumentNullException("exceptionToFormat");
            }
            Type type = exceptionToFormat.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] infoArray = properties;
            int index = 0;
        Label_0031:
            if (index >= infoArray.Length)
            {
                foreach (FieldInfo info2 in fields)
                {
                    try
                    {
                        fieldAccessFailed = info2.GetValue(exceptionToFormat);
                    }
                    catch (TargetInvocationException)
                    {
                        fieldAccessFailed = Messages.FieldAccessFailed;
                    }
                    WriteFieldInfo(info2, fieldAccessFailed);
                }
            }
            else
            {
                PropertyInfo propertyInfo = infoArray[index];
                if ((propertyInfo.CanRead && (IgnoredProperties.IndexOf(propertyInfo.Name) == -1)) && (propertyInfo.GetIndexParameters().Length == 0))
                {
                    try
                    {
                        fieldAccessFailed = propertyInfo.GetValue(exceptionToFormat, null);
                    }
                    catch (TargetInvocationException)
                    {
                        fieldAccessFailed = Messages.PropertyAccessFailed;
                    }
                    WritePropertyInfo(propertyInfo, fieldAccessFailed);
                }
                index++;
                goto Label_0031;
            }
        }

        protected abstract void WriteSource(string source);
        protected abstract void WriteStackTrace(string stackTrace);
    }
}

