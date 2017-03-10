namespace UIShell.OSGi.NDigester
{
    using System.Collections.Generic;

    internal interface IRules
    {
        void Add(string pattern, Rule rule);
        void Clear();
        IList<Rule> GetRules();
        IList<Rule> Match(string pattern);
        IList<Rule> Match(string namespaceURI, string pattern);

        Digester Digester { get; set; }

        string NamespaceURI { get; set; }
    }
}

