namespace UIShell.OSGi.NDigester
{
    internal abstract class RuleSet
    {
        protected string namespaceURI;

        protected RuleSet()
        {
        }

        public abstract void AddRules(Digester digester);

        public string NamespaceURI => namespaceURI;
    }
}

