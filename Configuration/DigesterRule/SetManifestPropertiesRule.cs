namespace UIShell.OSGi.Configuration.DigesterRule
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;
    using BundleManifest;
    using NDigester;
    using Utility;

    internal class SetManifestPropertiesRule : SetPropertiesRule
    {
        public override void OnBegin()
        {
            object obj2 = digester.Peek();
            string symbolicName = string.Empty;
            if (obj2 is BundleData)
            {
                symbolicName = base.digester.Attributes["SymbolicName"];
            }
            else if (base.digester.Root is BundleData)
            {
                symbolicName = (base.digester.Root as BundleData).SymbolicName;
            }
            using (IEnumerator enumerator = base.digester.Attributes.Keys.GetEnumerator())
            {
            Label_0071:
                if (!enumerator.MoveNext())
                {
                    return;
                }
                string current = (string) enumerator.Current;
                string str3 = base.digester.Attributes[current];
                PropertyInfo property = obj2.GetType().GetProperty(current, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    string name = char.ToUpper(current[0]) + current.Substring(1);
                    property = obj2.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                }
                if (property == null)
                {
                    goto Label_0191;
                }
                object obj3 = null;
                if (property.PropertyType.IsEnum)
                {
                    try
                    {
                        obj3 = Enum.Parse(property.PropertyType, str3, true);
                    }
                    catch (Exception exception)
                    {
                        FileLogUtility.Error(exception);
                    }
                }
                else
                {
                    try
                    {
                        obj3 = TypeConverterUtility.ConvertTo(str3, property.PropertyType);
                    }
                    catch (Exception exception2)
                    {
                        FileLogUtility.Error(exception2);
                    }
                }
                goto Label_01B2;
            Label_013B:
                if (!string.IsNullOrEmpty(str3))
                {
                    FileLogUtility.Warn(string.Format(Messages.ManifestSectionInvalid, new object[] { str3, current, base.digester.ElementName, symbolicName }));
                    goto Label_0071;
                }
            Label_0181:
                property.SetValue(obj2, obj3, null);
                goto Label_0071;
            Label_0191:
                FileLogUtility.Warn(string.Format(Messages.ManifestSectionNotRecognized, current, base.digester.ElementName, symbolicName));
                goto Label_0071;
            Label_01B2:
                if (obj3 != null)
                {
                    goto Label_0181;
                }
                goto Label_013B;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("SetManifestPropertiesRule[");
            builder.Append("]");
            return builder.ToString();
        }
    }
}

