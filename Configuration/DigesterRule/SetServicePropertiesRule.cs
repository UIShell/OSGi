namespace UIShell.OSGi.Configuration.DigesterRule
{
    using System;
    using System.Text;
    using BundleManifest;
    using NDigester;
    using Utility;

    internal class SetServicePropertiesRule : SetPropertiesRule
    {
        public override void OnBegin()
        {
            var data = digester.Peek() as ServiceData;
            if (data == null)
            {
                throw new NotSupportedException("the top element must be an instance of ServiceData,but it is " + base.digester.Peek().GetType().ToString());
            }
            var symbolicName = string.Empty;
            if (digester.Root is BundleData)
            {
                symbolicName = (digester.Root as BundleData).SymbolicName;
            }
            foreach (string str2 in digester.Attributes.Keys)
            {
                string str3 = digester.Attributes[str2];
                string str4 = str2.ToLower();
                if (str4 != null)
                {
                    if (str4 != "interface")
                    {
                        if (str4 != "type")
                        {
                            goto Label_00F2;
                        }
                        data.Type = str3;
                    }
                    else
                    {
                        data.Interfaces = str3.Split(new char[] { ';' });
                    }
                    continue;
                }
            Label_00F2:
                FileLogUtility.Warn(string.Format(Messages.ManifestSectionNotRecognized, str2, base.digester.ElementName, symbolicName));
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("SetServicePropertiesRule[");
            builder.Append("]");
            return builder.ToString();
        }
    }
}

