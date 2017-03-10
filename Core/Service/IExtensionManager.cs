namespace UIShell.OSGi.Core.Service
{
    using System;
    using System.Collections.Generic;
    using OSGi;

    public interface IExtensionManager
    {
        event EventHandler<ExtensionEventArgs> ExtensionChanged;

        event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged;

        void AddExtension(string point, Extension extension);
        ExtensionPoint GetExtensionPoint(string point);
        List<ExtensionPoint> GetExtensionPoints();
        List<ExtensionPoint> GetExtensionPoints(IBundle bundle);
        List<Extension> GetExtensions(string extensionPoint);
        void RemoveExtension(Extension extension);
    }
}

