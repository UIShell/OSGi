namespace UIShell.OSGi
{
    using System;
    using System.Runtime.Serialization;

    public class IllegalStateException : Exception
    {
        public IllegalStateException()
        {
        }

        public IllegalStateException(string message)
            : base(message)
        {
        }

        protected IllegalStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IllegalStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

