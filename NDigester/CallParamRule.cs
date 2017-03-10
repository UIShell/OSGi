namespace UIShell.OSGi.NDigester
{
    using System.Text;

    internal class CallParamRule : Rule
    {
        private string _attribute;
        private string _body;
        private int _paramIndex;

        public CallParamRule(int paramIndex) : this(paramIndex, null)
        {
        }

        public CallParamRule(int paramIndex, string attribute)
        {
            _paramIndex = paramIndex;
            _attribute = attribute;
        }

        public override void OnBegin()
        {
            if (_attribute != null)
            {
                _body = Digester.Attributes[_attribute];
            }
        }

        public override void OnBody()
        {
            if (_attribute == null)
            {
                _body = Digester.Body;
            }
        }

        public override void OnEnd()
        {
            Digester.PeekParameters()[_paramIndex] = _body;
        }

        public override void OnFinish()
        {
            _body = null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("CallParamRule[");
            builder.Append("paramIndex=");
            builder.Append(_paramIndex);
            builder.Append(", attributeName=");
            builder.Append(_attribute);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

