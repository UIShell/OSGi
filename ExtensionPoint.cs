namespace UIShell.OSGi
{
    using System.Collections.Generic;
    using Core.Service;
    using Utility;

    public class ExtensionPoint
    {
        private IExtensionManager _extensionManager;

        public IList<Extension> Extensions => _extensionManager.GetExtensions(Point);

        public IBundle Owner { get; internal set; }

        public string Point { get; internal set; }

        public string Schema { get; internal set; }

        internal ExtensionPoint(IExtensionManager extensionManager)
        {
            AssertUtility.ArgumentNotNull(extensionManager, "extensionManager");
            _extensionManager = extensionManager;
        }

        public void AddExtension(Extension extension)
        {
            _extensionManager.AddExtension(Point, extension);
        }

        public void RemoveExtension(Extension extension)
        {
            _extensionManager.RemoveExtension(extension);
        }
    }
}

