namespace UIShell.OSGi.Loader
{
    using System.Reflection;
    using Dependency.Metadata;

    internal class AssemblyEntry
    {
        public Assembly Assembly { get; set; }

        public string AssemblyPath { get; set; }

        public IAssemblyMetadata ProvidedBy { get; set; }
    }
}

