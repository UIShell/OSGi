namespace UIShell.OSGi.NDigester
{
    internal interface IRuleSet
    {
        void AddRules(Digester digester);

        string NamespaceURI { get; }
    }
}

