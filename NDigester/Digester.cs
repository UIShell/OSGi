namespace UIShell.OSGi.NDigester
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal class Digester
    {
        private NameValueCollection attributes = new NameValueCollection();
        private Stack<StringBuilder> bodies = new Stack<StringBuilder>();
        private string body;
        private string match = "";
        private Stack<string[]> parameters = new Stack<string[]>();
        private XmlReader reader;
        private object root;
        private IRules rules;
        private Stack<object> stack = new Stack<object>();

        public void AddCallMethod(string pattern, string methodName)
        {
            AddRule(pattern, new CallMethodRule(methodName));
        }

        public void AddCallMethod(string pattern, string methodName, int paramCount)
        {
            AddRule(pattern, new CallMethodRule(methodName, paramCount));
        }

        public void AddCallMethod(string pattern, string methodName, int paramCount, params object[] paramTypes)
        {
            AddRule(pattern, new CallMethodRule(methodName, paramCount, paramTypes));
        }

        public void AddCallParam(string pattern, int paramIndex)
        {
            AddRule(pattern, new CallParamRule(paramIndex));
        }

        public void AddCallParam(string pattern, int paramIndex, string attribute)
        {
            AddRule(pattern, new CallParamRule(paramIndex, attribute));
        }

        public void AddObjectCreate(string pattern, string typeName)
        {
            AddRule(pattern, new ObjectCreateRule(typeName));
        }

        public void AddObjectCreate(string pattern, Type type)
        {
            AddRule(pattern, new ObjectCreateRule(type));
        }

        public void AddObjectCreate(string pattern, string typeName, string attribute)
        {
            AddRule(pattern, new ObjectCreateRule(typeName, attribute));
        }

        public void AddObjectCreate(string pattern, Type type, string attribute)
        {
            AddRule(pattern, new ObjectCreateRule(type, attribute));
        }

        public void AddPropertySetter(string pattern)
        {
            AddRule(pattern, new PropertySetterRule());
        }

        public void AddPropertySetter(string pattern, string propertyName)
        {
            AddRule(pattern, new PropertySetterRule(propertyName));
        }

        public void AddRule(string pattern, Rule rule)
        {
            rule.Digester = this;
            Rules.Add(pattern, rule);
        }

        public void AddRuleSet(Type ruleSetType)
        {
            AddRuleSet(Activator.CreateInstance(ruleSetType) as RuleSet);
        }

        public void AddRuleSet(RuleSet ruleSet)
        {
            string namespaceURI = Rules.NamespaceURI;
            string str2 = ruleSet.NamespaceURI;
            Rules.NamespaceURI = str2;
            ruleSet.AddRules(this);
            Rules.NamespaceURI = namespaceURI;
        }

        public void AddSetNext(string pattern, string methodName)
        {
            AddRule(pattern, new SetNextRule(methodName));
        }

        public void AddSetProperties(string pattern)
        {
            AddRule(pattern, new SetPropertiesRule());
        }

        public void AddSetProperty(string pattern, string name, string value)
        {
            AddRule(pattern, new SetPropertyRule(name, value));
        }

        public void AddSetRoot(string pattern, string methodName)
        {
            AddRule(pattern, new SetRootRule(methodName));
        }

        public void AddSetTop(string pattern, string methodName)
        {
            AddRule(pattern, new SetTopRule(methodName));
        }

        public void Clear()
        {
            match = "";
            bodies.Clear();
            parameters.Clear();
            stack.Clear();
        }

        public void Configure(TextReader reader)
        {
            Configure(new XmlTextReader(reader));
        }

        public void Configure(string path)
        {
            Configure(new StreamReader(path));
        }

        public void Configure(XmlNode node)
        {
            Configure(new XmlNodeReader(node));
        }

        public void Configure(XmlReader reader)
        {
            Digester digester = new Digester();
            digester.AddRule("*/pattern", new PatternRule("value"));
            digester.AddRule("*/object-create-rule", new RuleFactoryRule(this, typeof(ObjectCreateRule), new object[] { "typeName", typeof(string), "attribute", typeof(string) }));
            digester.AddRule("*/call-method-rule", new RuleFactoryRule(this, typeof(CallMethodRule), new object[] { "methodName", typeof(string), "paramCount", typeof(int) }));
            digester.AddRule("*/call-param-rule", new RuleFactoryRule(this, typeof(CallParamRule), new object[] { "paramIndex", typeof(int), "attribute", typeof(string) }));
            digester.AddRule("*/set-properties-rule", new RuleFactoryRule(this, typeof(SetPropertiesRule), new object[0]));
            digester.AddRule("*/set-property-rule", new RuleFactoryRule(this, typeof(SetPropertyRule), new object[] { "name", typeof(string), "value", typeof(string) }));
            digester.AddRule("*/set-top-rule", new RuleFactoryRule(this, typeof(SetTopRule), new object[] { "methodName", typeof(string) }));
            digester.AddRule("*/set-next-rule", new RuleFactoryRule(this, typeof(SetNextRule), new object[] { "methodName", typeof(string) }));
            digester.AddRule("*/property-setter-rule", new RuleFactoryRule(this, typeof(PropertySetterRule), new object[] { "propertyName", typeof(string) }));
            digester.Parse(reader);
        }

        public object Parse(TextReader reader) =>
            Parse(new XmlTextReader(reader));

        public object Parse(string path) =>
            Parse(new XmlTextReader(new StreamReader(path)));

        public object Parse(XmlNode node) =>
            Parse(new XmlNodeReader(node));

        public object Parse(XmlReader reader)
        {
            this.reader = reader;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    bool flag;
                    if (!(flag = reader.IsEmptyElement))
                    {
                        bodies.Push(new StringBuilder());
                    }
                    else
                    {
                        body = null;
                    }
                    StringBuilder builder = new StringBuilder(match);
                    if (match.Length > 0)
                    {
                        builder.Append("/");
                    }
                    if ((reader.LocalName != null) && (reader.LocalName.Length <= 1))
                    {
                        builder.Append(reader.LocalName);
                    }
                    else
                    {
                        builder.Append(reader.Name);
                    }
                    match = builder.ToString();
                    string namespaceURI = reader.NamespaceURI;
                    attributes.Clear();
                    while (reader.MoveToNextAttribute())
                    {
                        attributes[reader.Name] = reader.Value;
                    }
                    IList<Rule> list = Rules.Match(namespaceURI, match);
                    if ((list != null) && (list.Count > 0))
                    {
                        foreach (Rule rule in list)
                        {
                            rule.OnBegin();
                        }
                    }
                    if (flag)
                    {
                        if (list != null)
                        {
                            for (int i = list.Count - 1; i >= 0; i--)
                            {
                                list[i].OnEnd();
                            }
                        }
                        int length = match.LastIndexOf("/");
                        if (length >= 0)
                        {
                            match = match.Substring(0, length);
                        }
                        else
                        {
                            match = "";
                        }
                    }
                }
                if ((reader.NodeType == XmlNodeType.Text) || (reader.NodeType == XmlNodeType.CDATA))
                {
                    bodies.Peek().Append(reader.Value);
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    body = bodies.Pop().ToString();
                    IList<Rule> list2 = null;
                    if (body != null)
                    {
                        list2 = Rules.Match(reader.NamespaceURI, match);
                        if ((list2 != null) && (list2.Count > 0))
                        {
                            foreach (Rule rule3 in list2)
                            {
                                rule3.OnBody();
                            }
                        }
                    }
                    if (list2 != null)
                    {
                        for (int j = list2.Count - 1; j >= 0; j--)
                        {
                            list2[j].OnEnd();
                        }
                    }
                    int num2 = match.LastIndexOf("/");
                    if (num2 >= 0)
                    {
                        match = match.Substring(0, num2);
                    }
                    else
                    {
                        match = "";
                    }
                }
            }
            while (Depth > 1)
            {
                Pop();
            }
            foreach (Rule rule2 in Rules.GetRules())
            {
                rule2.OnFinish();
            }
            Clear();
            return root;
        }

        public object Peek()
        {
            if (stack.Count <= 0)
            {
                return null;
            }
            return stack.Peek();
        }

        public string[] PeekParameters()
        {
            if (parameters.Count <= 0)
            {
                return null;
            }
            return parameters.Peek();
        }

        public object Pop()
        {
            if (stack.Count <= 0)
            {
                return null;
            }
            return stack.Pop();
        }

        public string[] PopParameters()
        {
            if (parameters.Count <= 0)
            {
                return null;
            }
            return parameters.Pop();
        }

        public void Push(object value)
        {
            if (stack.Count == 0)
            {
                root = value;
            }
            stack.Push(value);
        }

        public void PushParameters(string[] value)
        {
            parameters.Push(value);
        }

        public NameValueCollection Attributes =>
            attributes;

        public string Body =>
            body;

        public int Depth =>
            stack.Count;

        public string ElementName
        {
            get
            {
                string match = this.match;
                int num = match.LastIndexOf('/');
                if (num >= 0)
                {
                    match = match.Substring(num + 1);
                }
                return match;
            }
        }

        public string Match =>
            match;

        public object Root =>
            root;

        public IRules Rules
        {
            get
            {
                if (rules == null)
                {
                    rules = new Rules();
                    rules.Digester = this;
                }
                return rules;
            }
            set
            {
                rules = value;
                rules.Digester = this;
            }
        }
    }
}

