namespace UIShell.OSGi.Configuration.BundleManifest
{
    using System.Collections.Generic;

    public class RuntimeData
    {
        public RuntimeData()
        {
            Dependencies = new List<DependencyData>();
            Assemblies = new List<AssemblyData>();
        }

        public void AddAssembly(AssemblyData assembly)
        {
            Assemblies.Add(assembly);
        }

        public void AddDependency(DependencyData newItem)
        {
            Dependencies.Add(newItem);
        }

        public List<AssemblyData> Assemblies { get; private set; }

        public List<DependencyData> Dependencies { get; private set; }
    }
}

