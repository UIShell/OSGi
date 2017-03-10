namespace UIShell.OSGi.NDigester
{
    using System.Reflection;
    using System.Text;

    internal class SetRootRule : Rule
    {
        private string _methodName;

        public SetRootRule(string methodName)
        {
            _methodName = methodName;
        }

        public override void OnEnd()
        {
            object obj2 = Digester.Peek();
            object root = Digester.Root;
            MethodInfo method = root.GetType().GetMethod(_methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(root, new object[] { obj2 });
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder("SetRootRule[");
            builder.Append("methodName=");
            builder.Append(_methodName);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

