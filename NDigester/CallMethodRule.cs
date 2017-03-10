namespace UIShell.OSGi.NDigester
{
    using System;
    using System.Reflection;
    using System.Text;
    using Utility;

    internal class CallMethodRule : Rule
    {
        private string body;
        private string methodName;
        private int paramCount;
        private Type[] paramTypes;

        public CallMethodRule(string methodName) : this(methodName, -1, null)
        {
        }

        public CallMethodRule(string methodName, int paramCount) : this(methodName, paramCount, null)
        {
        }

        public CallMethodRule(string methodName, int paramCount, params object[] paramTypes)
        {
            this.paramCount = -1;
            this.methodName = methodName;
            this.paramCount = paramCount;
            if (paramTypes != null)
            {
                this.paramTypes = new Type[paramTypes.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    object obj2 = paramTypes[i];
                    if (obj2 is string)
                    {
                        this.paramTypes[i] = Type.GetType((string) obj2);
                    }
                    else
                    {
                        if (!(obj2 is Type))
                        {
                            throw new Exception("You must specify only objects of type \"System.String\" and \"System.Type\" here!");
                        }
                        this.paramTypes[i] = (Type) obj2;
                    }
                }
            }
        }

        public override void OnBegin()
        {
            if (paramCount > 0)
            {
                string[] strArray = new string[paramCount];
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = null;
                }
                Digester.PushParameters(strArray);
            }
        }

        public override void OnBody()
        {
            if (paramCount == 0)
            {
                body = Digester.Body.Trim();
            }
        }

        public override void OnEnd()
        {
            string[] strArray = null;
            if (paramCount < 0)
            {
                strArray = new string[0];
            }
            else if (paramCount == 0)
            {
                strArray = new string[] { body };
                if (body == null)
                {
                    return;
                }
            }
            else if (paramCount > 0)
            {
                strArray = Digester.PopParameters();
                if ((paramCount == 1) && (strArray[0] == null))
                {
                    return;
                }
            }
            object obj2 = Digester.Peek();
            MethodInfo method = null;
            if (paramTypes != null)
            {
                method = obj2.GetType().GetMethod(methodName, paramTypes);
            }
            else
            {
                method = obj2.GetType().GetMethod(methodName, new Type[0]);
            }
            if (method != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                object[] objArray = new object[strArray.Length];
                for (int i = 0; i < objArray.Length; i++)
                {
                    objArray[i] = TypeConverterUtility.ConvertTo(strArray[i], parameters[i].ParameterType);
                }
                method.Invoke(obj2, objArray);
                body = null;
            }
        }

        public override void OnFinish()
        {
            body = null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("CallMethodRule[");
            builder.Append("methodName=");
            builder.Append(methodName);
            builder.Append(", paramCount=");
            builder.Append(paramCount);
            builder.Append(", paramTypes={");
            if (paramTypes != null)
            {
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(paramTypes[i].Name);
                }
            }
            builder.Append("}");
            builder.Append("]");
            return builder.ToString();
        }
    }
}

