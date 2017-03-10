namespace UIShell.OSGi.Configuration.BundleManifest
{
    using System.Collections.Generic;
    using System.Xml;

    public class ExtensionPointData
    {
        public ExtensionPointData()
        {
            ChildNodes = new List<XmlNode>();
        }

        public List<XmlNode> ChildNodes { get; private set; }

        public string Point { get; set; }

        public string Schema { get; set; }
    }
}

