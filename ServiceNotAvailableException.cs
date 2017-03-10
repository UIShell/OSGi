namespace UIShell.OSGi
{
    using System;
    using System.Runtime.Serialization;

    public class ServiceNotAvailableException : Exception
    {
        protected ServiceNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ServiceNotAvailableException(string serviceNotAvailableMessage, Exception innerException)
            : base(serviceNotAvailableMessage, innerException)
        {
        }

        public ServiceNotAvailableException(string name, string application)
            : base($"Dependent service '{name}' in application '{application}' is not avaiable.")
        {
        }

        public ServiceNotAvailableException(string name, IBundle exceptionThrownBundle)
            : this(name, $"{exceptionThrownBundle.SymbolicName}, {exceptionThrownBundle.Version}")
        {
        }
    }
}

