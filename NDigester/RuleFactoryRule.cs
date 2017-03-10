namespace UIShell.OSGi.NDigester
{
    using System;
    using System.Collections;
    using UIShell.OSGi.Utility;

    internal class RuleFactoryRule : Rule
    {
        private Digester configDigester;
        private object[] prms;
        private Type ruleType;

        public RuleFactoryRule(Digester configDigester, Type ruleType, params object[] prms)
        {
            this.configDigester = configDigester;
            this.ruleType = ruleType;
            this.prms = prms;
        }

        public override void OnBegin()
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            if (prms != null)
            {
                for (int i = 0; i < prms.Length; i += 2)
                {
                    string str2 = (string)prms[i];
                    Type type = (Type)prms[i + 1];
                    string str3 = Digester.Attributes[str2];
                    if (str3 != null)
                    {
                        list2.Add(type);
                        list.Add(TypeConverterUtility.ConvertTo(str3, type));
                    }
                }
            }
            Type[] types = (Type[]) list2.ToArray(typeof(Type));
            object[] parameters = list.ToArray();
            Rule rule = (Rule)ruleType.GetConstructor(types).Invoke(parameters);
            string pattern = PatternRule.Pattern;
            if (Digester.Attributes["pattern"] != null)
            {
                if (pattern.Length > 0)
                {
                    pattern = pattern + "/";
                }
                pattern = pattern + Digester.Attributes["pattern"];
            }
            configDigester.AddRule(pattern, rule);
        }
    }
}

