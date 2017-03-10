namespace UIShell.OSGi
{
    using System;
    using System.Runtime.Serialization;

    public class BundleException : Exception
    {
        public BundleException()
        {
        }

        public BundleException(string message)
            : base(message)
        {
        }

        protected BundleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public BundleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

