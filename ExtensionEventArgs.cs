namespace UIShell.OSGi
{
    using System;
    using Core.Service;

    public class ExtensionEventArgs : EventArgs
    {
        internal ExtensionEventArgs(string extensionPoint, IExtensionManager extensionManager, CollectionChangedAction action)
        {
            ExtensionPoint = extensionPoint;
            ExtensionManager = extensionManager;
            Action = action;
        }

        public CollectionChangedAction Action { get; private set; }

        public IBundle Bundle => Extension?.Owner;

        public Extension Extension { get; internal set; }

        public IExtensionManager ExtensionManager { get; private set; }

        public string ExtensionPoint { get; private set; }
    }
}

