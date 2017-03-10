namespace UIShell.OSGi.Configuration.BundleManifest
{
    using System.Collections.Generic;
    using System.Xml;

    public class ExtensionData
    {
        public ExtensionData()
        {
            ChildNodes = new List<XmlNode>();
        }

        public List<XmlNode> ChildNodes { get; private set; }

        public string Point { get; set; }
    }
}

