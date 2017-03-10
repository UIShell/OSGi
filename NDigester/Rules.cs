namespace UIShell.OSGi.NDigester
{
    using System.Collections.Generic;

    internal class Rules : IRules
    {
        private Dictionary<string, IList<Rule>> _cache = new Dictionary<string, IList<Rule>>();
        private Digester _digester = null;
        private string _namespaceURI = string.Empty;
        private IList<Rule> _rules = new List<Rule>();

        public void Add(string pattern, Rule rule)
        {
            IList<Rule> list;
            if (!_cache.TryGetValue(pattern, out list))
            {
                list = new List<Rule>();
                _cache[pattern] = list;
            }
            list.Add(rule);
            _rules.Add(rule);
            if (_namespaceURI != null)
            {
                rule.NamespaceURI = _namespaceURI;
            }
        }

        public void Clear()
        {
            _cache.Clear();
            _rules.Clear();
        }

        public IList<Rule> GetRules() =>
            _rules;

        protected IList<Rule> Lookup(string namespaceURI, string pattern)
        {
            IList<Rule> list = null;
            if (!_cache.TryGetValue(pattern, out list))
            {
                return null;
            }
            if ((namespaceURI == null) || (namespaceURI.Length == 0))
            {
                return list;
            }
            IList<Rule> list2 = new List<Rule>();
            foreach (Rule rule in list)
            {
                if ((namespaceURI == rule.NamespaceURI) || (rule.NamespaceURI == null))
                {
                    list2.Add(rule);
                }
            }
            return list2;
        }

        public IList<Rule> Match(string pattern) =>
            Match(null, pattern);

        public IList<Rule> Match(string namespaceURI, string pattern)
        {
            IList<Rule> list = Lookup(namespaceURI, pattern);
            if ((list == null) || (list.Count < 1))
            {
                string str = "";
                foreach (string str2 in _cache.Keys)
                {
                    if ((str2.StartsWith("*/") && pattern.EndsWith(str2.Substring(2))) && (str2.Length > str.Length))
                    {
                        list = Lookup(namespaceURI, str2);
                        str = str2;
                    }
                }
            }
            if (list == null)
            {
                list = new List<Rule>();
            }
            return list;
        }

        public Digester Digester
        {
            get { return _digester; }
            set
            {
                _digester = value;
            }
        }

        public string NamespaceURI
        {
            get { return _namespaceURI; }
            set
            {
                _namespaceURI = value;
            }
        }
    }
}

