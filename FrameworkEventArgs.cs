namespace UIShell.OSGi
{
    using System;

    public class FrameworkEventArgs : EventArgs
    {
        internal FrameworkEventArgs(FrameworkEventType type, object target)
            : this(type, target, null)
        {
        }

        internal FrameworkEventArgs(FrameworkEventType type, object target, object data)
        {
            if (target == null)
            {
                throw new ArgumentNullException();
            }
            EventType = type;
            Target = target;
            Data = data;
        }

        public object Data { get; private set; }

        public FrameworkEventType EventType { get; private set; }

        public object Target { get; private set; }
    }
}

