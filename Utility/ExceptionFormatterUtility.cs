namespace UIShell.OSGi.Utility
{
    using System;
    using System.IO;
    using System.Text;

    public class ExceptionFormatterUtility
    {
        public static string Format(Exception exception)
        {
            StringWriter writer = null;
            StringBuilder stringBuilder = null;
            try
            {
                writer = new StringWriter();
                new TextExceptionFormatter(writer, exception).Format();
                stringBuilder = writer.GetStringBuilder();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return stringBuilder.ToString();
        }
    }
}

