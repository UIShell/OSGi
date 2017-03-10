namespace UIShell.OSGi.Configuration.DigesterRule
{
    using System.Reflection;
    using System.Text;
    using NDigester;

    internal class NextPropertySetterRule : Rule
    {
        private string propertyName;

        public NextPropertySetterRule() : this(null)
        {
        }

        public NextPropertySetterRule(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public override void OnEnd()
        {
            object obj2 = base.digester.Pop();
            string propertyName = this.propertyName;
            if (propertyName == null)
            {
                propertyName = base.digester.ElementName;
            }
            object obj3 = base.digester.Peek();
            PropertyInfo property = obj3.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                property.SetValue(obj3, obj2, null);
            }
            base.digester.Push(obj2);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("NextPropertySetterRule[");
            builder.Append("propertyName=");
            builder.Append(this.propertyName);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

