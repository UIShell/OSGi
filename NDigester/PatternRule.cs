namespace UIShell.OSGi.NDigester
{
    using System.Collections.Specialized;

    internal class PatternRule : Rule
    {
        private string attribute;
        private static StringCollection path = new StringCollection();

        public PatternRule(string attribute)
        {
            this.attribute = attribute;
            path = new StringCollection();
        }

        public override void OnBegin()
        {
            path.Add(Digester.Attributes[attribute]);
        }

        public override void OnEnd()
        {
            path.RemoveAt(path.Count - 1);
        }

        public override void OnFinish()
        {
            path = null;
        }

        public static string Pattern
        {
            get
            {
                string[] array = new string[path.Count];
                path.CopyTo(array, 0);
                return string.Join("/", array);
            }
        }
    }
}

