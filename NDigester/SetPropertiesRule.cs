namespace UIShell.OSGi.NDigester
{
    using System;
    using System.Reflection;
    using System.Text;
    using Utility;

    internal class SetPropertiesRule : Rule
    {
        public override void OnBegin()
        {
            object obj2 = base.Digester.Peek();
            foreach (string str in base.Digester.Attributes.Keys)
            {
                string str2 = base.Digester.Attributes[str];
                PropertyInfo property = obj2.GetType().GetProperty(str, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    string name = char.ToUpper(str[0]) + str.Substring(1);
                    property = obj2.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                }
                if (property != null)
                {
                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(obj2, Enum.Parse(property.PropertyType, str2, true), null);
                    }
                    else
                    {
                        property.SetValue(obj2, TypeConverterUtility.ConvertTo(str2, property.PropertyType), null);
                    }
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder("SetPropertiesRule[");
            builder.Append("]");
            return builder.ToString();
        }
    }
}

