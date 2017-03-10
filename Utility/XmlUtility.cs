namespace UIShell.OSGi.Utility
{
    using System;
    using System.Xml;

    internal static class XmlUtility
    {
        public static XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument doc, string xmlNamespace)
        {
            XmlNode rootNode = GetRootNode(doc);
            if (rootNode != null)
            {
                return CreateXmlNamespaceManager(rootNode, xmlNamespace);
            }
            return new XmlNamespaceManager(doc.NameTable);
        }

        public static XmlNamespaceManager CreateXmlNamespaceManager(XmlNode node, string xmlNamespace)
        {
            AssertUtility.NotNull(node);
            XmlNamespaceManager manager = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            if ((string.IsNullOrEmpty(xmlNamespace) ? 0 : 1) != 0)
            {
                if (xmlNamespace != "urn:uiosp-bundle-manifest-2.0")
                {
                    throw new Exception("The xmlns attribute of Bundle in Manifest.xml must be urn:uiosp-bundle-manifest-2.0");
                }
                manager.AddNamespace("ebm2", "urn:uiosp-bundle-manifest-2.0");
            }
            return manager;
        }

        public static string GetNamespace(XmlDocument doc) => 
            GetNamespace(GetRootNode(doc));

        public static string GetNamespace(XmlNode node)
        {
            if (node != null)
            {
                XmlAttribute attribute = node.Attributes["xmlns"];
                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            return string.Empty;
        }

        private static XmlNode GetRootNode(XmlNode doc)
        {
            if (doc.ChildNodes.Count <= 0)
            {
                return null;
            }
            return doc.ChildNodes[doc.ChildNodes.Count - 1];
        }
    }
}

