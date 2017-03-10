namespace UIShell.OSGi.NDigester
{
    internal abstract class Rule
    {
        public Rule()
        {
            Digester = null;
            NamespaceURI = string.Empty;
        }

        public Rule(Digester digester)
        {
            Digester = digester;
        }

        public virtual void OnBegin()
        {
        }

        public virtual void OnBody()
        {
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnFinish()
        {
        }

        public Digester Digester { get; set; }

        public string NamespaceURI { get; set; }
    }
}

