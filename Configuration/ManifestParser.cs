namespace UIShell.OSGi.Configuration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using BundleManifest;
    using DigesterRule;
    using NDigester;
    using Utility;

    public class ManifestParser
    {
        private static Digester _bundleDataDigester;

        static ManifestParser()
        {
            CreateBundleDataDigester();
        }

        public static BundleData CreateBundleData(string manifestFile)
        {
            FileStream manifestFileStream = null;
            BundleData data;
            try
            {
                manifestFileStream = File.OpenRead(manifestFile);
                data = CreateBundleData(manifestFile, manifestFileStream);
            }
            finally
            {
                if (manifestFileStream != null)
                {
                    manifestFileStream.Close();
                }
            }
            return data;
        }

        public static BundleData CreateBundleData(string manifestFile, Stream manifestFileStream)
        {
            BundleData data2;
            Stopwatch stopwatch = new Stopwatch();
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, GetEncoding(manifestFileStream));
            try
            {
                stopwatch.Start();
                string content = new StreamReader(manifestFileStream).ReadToEnd();
                content = ReplaceConstants(manifestFile, content);
                writer.Write(content);
                writer.Flush();
                stream.Seek(0L, SeekOrigin.Begin);
                BundleData data = (BundleData) _bundleDataDigester.Parse(new XmlTextReader(stream));
                stream.Seek(0L, SeekOrigin.Begin);
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                string xmlNamespace = XmlUtility.GetNamespace(doc);
                XmlNamespaceManager xmlnsMgr = XmlUtility.CreateXmlNamespaceManager(doc, xmlNamespace);
                bool withNamespace = !string.IsNullOrEmpty(xmlNamespace);
                data.ExtensionPoints.ForEach(delegate (ExtensionPointData item) {
                    string xpath = string.Format((withNamespace ? "/ebm2:Bundle/ebm2:ExtensionPoint" : "/Bundle/ExtensionPoint") + "[@Point='{0}']", item.Point);
                    GetChildNode(doc.SelectNodes(xpath, xmlnsMgr), childNode => item.ChildNodes.Add(childNode));
                });
                data.Extensions.ForEach(delegate (ExtensionData item) {
                    string xpath = string.Format((withNamespace ? "/ebm2:Bundle/ebm2:Extension" : "/Bundle/Extension") + "[@Point='{0}']", item.Point);
                    GetChildNode(doc.SelectNodes(xpath, xmlnsMgr), childNode => item.ChildNodes.Add(childNode));
                });
                data2 = data;
            }
            finally
            {
                stopwatch.Stop();
                manifestFileStream.Close();
                stream.Close();
                writer.Close();
                FileLogUtility.Verbose(string.Format(Messages.ParseManifestTimeCounter, stopwatch.ElapsedMilliseconds, manifestFile));
            }
            return data2;
        }

        private static void CreateBundleDataDigester()
        {
            if (_bundleDataDigester == null)
            {
                _bundleDataDigester = new Digester();
                _bundleDataDigester.Rules.NamespaceURI = "urn:uiosp-bundle-manifest-2.0";
                string pattern = string.Empty;
                pattern = "Bundle";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(BundleData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                pattern = "Bundle/BundleInfo";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(BundleInfoData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                _bundleDataDigester.AddRule(pattern, new NextPropertySetterRule("BundleInfo"));
                pattern = "Bundle/Activator";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(ActivatorData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                _bundleDataDigester.AddRule(pattern, new NextPropertySetterRule("Activator"));
                pattern = "Bundle/Runtime";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(RuntimeData));
                _bundleDataDigester.AddRule(pattern, new NextPropertySetterRule("Runtime"));
                pattern = "Bundle/Runtime/Assembly";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(AssemblyData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                _bundleDataDigester.AddSetNext(pattern, "AddAssembly");
                pattern = "Bundle/Runtime/Dependency";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(DependencyData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                _bundleDataDigester.AddSetNext(pattern, "AddDependency");
                pattern = "Bundle/Services/Service";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(ServiceData));
                _bundleDataDigester.AddRule(pattern, new SetServicePropertiesRule());
                _bundleDataDigester.AddSetNext(pattern, "AddService");
                pattern = "Bundle/ExtensionPoint";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(ExtensionPointData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                _bundleDataDigester.AddSetNext(pattern, "AddExtensionPoint");
                pattern = "Bundle/Extension";
                _bundleDataDigester.AddObjectCreate(pattern, typeof(ExtensionData));
                _bundleDataDigester.AddRule(pattern, new SetManifestPropertiesRule());
                _bundleDataDigester.AddSetNext(pattern, "AddExtension");
            }
        }

        private static void GetChildNode(XmlNodeList nodes, Action<XmlNode> acion)
        {
            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    acion(node2);
                }
            }
        }

        private static Encoding GetEncoding(Stream manifestFileStream)
        {
            Encoding encoding = Encoding.Default;
            byte[] buffer = new byte[5];
            manifestFileStream.Read(buffer, 0, 5);
            manifestFileStream.Seek(0L, SeekOrigin.Begin);
            if (((buffer[0] == 0xef) && (buffer[1] == 0xbb)) && (buffer[2] == 0xbf))
            {
                return Encoding.UTF8;
            }
            if ((buffer[0] == 0xfe) && (buffer[1] == 0xff))
            {
                return Encoding.Unicode;
            }
            if (((buffer[0] == 0) && (buffer[1] == 0)) && ((buffer[2] == 0xfe) && (buffer[3] == 0xff)))
            {
                return Encoding.UTF32;
            }
            if (((buffer[0] == 0x2b) && (buffer[1] == 0x2f)) && (buffer[2] == 0x76))
            {
                encoding = Encoding.UTF7;
            }
            return encoding;
        }

        private static string ReplaceConstants(string manifestFile, string content)
        {
            string newValue = Directory.GetParent(manifestFile).FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string str2 = AppDomain.CurrentDomain.BaseDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string str3 = string.Empty;
            if (newValue.StartsWith(str2))
            {
                str3 = newValue.Remove(0, str2.Length);
            }
            StringBuilder builder = new StringBuilder(content);
            builder.Replace("{BundleLocation}", newValue).Replace("{BundleAbsolutePath}", newValue).Replace("{BaseDirectory}", str2);
            if (!string.IsNullOrEmpty(str3))
            {
                builder.Replace("{BundleRelativePath}", str3);
            }
            return builder.ToString();
        }
    }
}

