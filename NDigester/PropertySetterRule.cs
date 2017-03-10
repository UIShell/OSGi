namespace UIShell.OSGi.NDigester
{
    using System.Reflection;
    using System.Text;
    using Utility;

    internal class PropertySetterRule : Rule
    {
        private string body;
        private string propertyName;

        public PropertySetterRule() : this(null)
        {
        }

        public PropertySetterRule(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public override void OnBody()
        {
            body = Digester.Body.Trim();
        }

        public override void OnEnd()
        {
            string propertyName = this.propertyName;
            if (propertyName == null)
            {
                propertyName = Digester.ElementName;
            }
            object obj2 = Digester.Peek();
            PropertyInfo property = obj2.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                property.SetValue(obj2, TypeConverterUtility.ConvertTo(body, property.PropertyType), null);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("PropertySetterRule[");
            builder.Append("propertyName=");
            builder.Append(propertyName);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

