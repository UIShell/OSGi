namespace UIShell.OSGi
{
    using System.Collections.Generic;
    using System.Xml;

    public class Extension
    {
        public Extension()
        {
            Data = new List<XmlNode>();
        }

        public List<XmlNode> Data { get; internal set; }

        public IBundle Owner { get; internal set; }
    }
}

