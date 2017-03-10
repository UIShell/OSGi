namespace UIShell.OSGi.NDigester
{
    using System.Reflection;
    using System.Text;

    internal class SetTopRule : Rule
    {
        private string _methodName;

        public SetTopRule(string methodName)
        {
            _methodName = methodName;
        }

        public override void OnEnd()
        {
            object obj2 = Digester.Pop();
            object obj3 = Digester.Peek();
            var method = obj2.GetType().GetMethod(_methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(obj2, new object[] { obj3 });
            }
            Digester.Push(obj2);
        }

        public override string ToString()
        {
            var builder = new StringBuilder("SetTopRule[");
            builder.Append("methodName=");
            builder.Append(_methodName);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

