namespace UIShell.OSGi.NDigester
{
    using System.Reflection;
    using System.Text;
    using Utility;

    internal class SetPropertyRule : Rule
    {
        private string _name;
        private string _value;

        public SetPropertyRule(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override void OnBegin()
        {
            string name = null;
            string str2 = null;
            for (int i = 0; i < Digester.Attributes.Count; i++)
            {
                string key = Digester.Attributes.GetKey(i);
                string str3 = Digester.Attributes[i];
                if (key == _name)
                {
                    name = str3;
                }
                else if (key == _value)
                {
                    str2 = str3;
                }
            }
            object obj2 = Digester.Peek();
            PropertyInfo property = obj2.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            property.SetValue(obj2, TypeConverterUtility.ConvertTo(str2, property.PropertyType), null);
        }

        public override string ToString()
        {
            var builder = new StringBuilder("SetPropertyRule[");
            builder.Append("name=");
            builder.Append(_name);
            builder.Append(", value=");
            builder.Append(_value);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

