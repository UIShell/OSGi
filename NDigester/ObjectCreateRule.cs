namespace UIShell.OSGi.NDigester
{
    using System;
    using System.Text;

    internal class ObjectCreateRule : Rule
    {
        private string attribute;
        private Type type;

        public ObjectCreateRule(string typeName) : this(typeName, null)
        {
        }

        public ObjectCreateRule(Type type) : this(type, null)
        {
        }

        public ObjectCreateRule(string typeName, string attribute)
        {
            type = Type.GetType(typeName, true, true);
            this.attribute = attribute;
        }

        public ObjectCreateRule(Type type, string attribute)
        {
            this.type = type;
            this.attribute = attribute;
        }

        public override void OnBegin()
        {
            if (attribute != null)
            {
                string typeName = Digester.Attributes[attribute];
                if (typeName != null)
                {
                    Type.GetType(typeName, true, true);
                }
            }
            object obj2 = Activator.CreateInstance(type);
            Digester.Push(obj2);
        }

        public override void OnEnd()
        {
            Digester.Pop();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("ObjectCreateRule[");
            builder.Append("typeName=");
            builder.Append(type.FullName);
            builder.Append(", attribute=");
            builder.Append(attribute);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

