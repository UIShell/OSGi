namespace UIShell.OSGi.NDigester
{
    using System.Reflection;
    using System.Text;

    internal class SetNextRule : Rule
    {
        private string methodName;

        public SetNextRule(string methodName)
        {
            this.methodName = methodName;
        }

        public override void OnEnd()
        {
            object obj2 = Digester.Pop();
            object obj3 = Digester.Peek();
            var method = obj3.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(obj3, new object[] { obj2 });
            }
            Digester.Push(obj2);
        }

        public override string ToString()
        {
            var builder = new StringBuilder("SetNextRule[");
            builder.Append("methodName=");
            builder.Append(methodName);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

