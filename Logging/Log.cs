namespace UIShell.OSGi.Logging
{
    public abstract class Log
    {
        private volatile bool isLogging = true;

        protected Log()
        {
        }

        public abstract void WriteLine(string message, LogLevel level);

        public virtual bool IsLogging
        {
            get { return isLogging; }
            set { isLogging = value; }
        }
    }
}

