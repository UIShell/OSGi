namespace UIShell.OSGi.Utility
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public class TextExceptionFormatter : ExceptionFormatter
    {
        private int _innerDepth;
        private readonly TextWriter _writer;

        public TextExceptionFormatter(TextWriter writer, Exception exception)
            : base(exception)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            _writer = writer;
        }

        protected virtual void Indent()
        {
            for (int i = 0; i < _innerDepth; i++)
            {
                Writer.Write("\t");
            }
        }

        private void IndentAndWriteLine(string format, params object[] arg)
        {
            Indent();
            Writer.WriteLine(format, arg);
        }

        protected override void WriteAdditionalInfo(NameValueCollection additionalInformation)
        {
            Writer.WriteLine(Messages.AdditionalInfo);
            Writer.WriteLine();
            foreach (string str in additionalInformation.AllKeys)
            {
                Writer.Write(str);
                Writer.Write(" : ");
                Writer.Write(additionalInformation[str]);
                Writer.Write("\n");
            }
        }

        protected override void WriteDateTime(DateTime utcNow)
        {
            string str = utcNow.ToLocalTime().ToString("G", DateTimeFormatInfo.InvariantInfo);
            Writer.WriteLine(str);
        }

        protected override void WriteDescription()
        {
            string str = string.Format(Messages.Culture, Messages.ExceptionWasCaught, new object[] { base.Exception.GetType().FullName });
            Writer.WriteLine(str);
            string str2 = new string('-', str.Length);
            Writer.WriteLine(str2);
        }

        protected override void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (outerException != null)
            {
                _innerDepth++;
                Indent();
                string innerException = Messages.InnerException;
                string str2 = new string('-', innerException.Length);
                Writer.WriteLine(innerException);
                Indent();
                Writer.WriteLine(str2);
                base.WriteException(exceptionToFormat, outerException);
                _innerDepth--;
            }
            else
            {
                base.WriteException(exceptionToFormat, outerException);
            }
        }

        protected override void WriteExceptionType(Type exceptionType)
        {
            IndentAndWriteLine(Messages.TypeString, new object[] { exceptionType.AssemblyQualifiedName });
        }

        protected override void WriteFieldInfo(FieldInfo fieldInfo, object value)
        {
            Indent();
            Writer.Write(fieldInfo.Name);
            Writer.Write(" : ");
            Writer.WriteLine(value);
        }

        protected override void WriteHelpLink(string helpLink)
        {
            IndentAndWriteLine(Messages.HelpLink, new object[] { helpLink });
        }

        protected override void WriteMessage(string message)
        {
            IndentAndWriteLine(Messages.Message, new object[] { message });
        }

        protected override void WritePropertyInfo(PropertyInfo propertyInfo, object value)
        {
            Indent();
            Writer.Write(propertyInfo.Name);
            Writer.Write(" : ");
            Writer.WriteLine(value);
        }

        protected override void WriteSource(string source)
        {
            IndentAndWriteLine(Messages.Source, new object[] { source });
        }

        protected override void WriteStackTrace(string stackTrace)
        {
            Indent();
            Writer.Write(Messages.StackTrace);
            Writer.Write(" : ");
            if ((stackTrace != null) && (stackTrace.Length != 0))
            {
                string str2 = stackTrace.Replace("\n", "\n" + new string('\t', _innerDepth));
                Writer.WriteLine(str2);
                Writer.WriteLine();
            }
            else
            {
                Writer.WriteLine(Messages.StackTraceUnavailable);
            }
        }

        public TextWriter Writer =>
            _writer;
    }
}

