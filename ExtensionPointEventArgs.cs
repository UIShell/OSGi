namespace UIShell.OSGi
{
    using System;
    using Core.Service;

    public class ExtensionPointEventArgs : EventArgs
    {
        public CollectionChangedAction Action { get; private set; }

        public IBundle Bundle => ExtensionPoint?.Owner;

        public IExtensionManager ExtensionManager { get; private set; }

        public ExtensionPoint ExtensionPoint { get; internal set; }

        internal ExtensionPointEventArgs(IExtensionManager extensionManager, CollectionChangedAction action)
        {
            ExtensionManager = extensionManager;
            Action = action;
        }
    }
}

