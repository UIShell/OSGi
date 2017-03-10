namespace UIShell.OSGi.Loader
{
    using System;
    using System.Reflection;
    using Dependency.Metadata;
    using OSGi;

    internal class AssemblyLoader : AbstractRuntimeLoader, IRuntimeLoader, IAssemblyLoader
    {
        public AssemblyLoader(IAssemblyMetadata import, BundleLoader load) : base(load)
        {
            if (import == null)
            {
                throw new ArgumentNullException();
            }
            Import = import;
        }

        private Assembly GetAssembly()
        {
            if (!Import.IsDuplicatedWithGlobalAssembly)
            {
                return Owner.Framework.ServiceContainer.GetFirstOrDefaultService<IAssemblyResolvingProvider>().ResolveAssembly(Import, Import.AssemblyName);
            }
            using (new UseDefaultClrLoaderBehavior())
            {
                return Assembly.Load(Import.AssemblyName);
            }
        }

        public override Type LoadClass(string className)
        {
            Assembly assembly = GetAssembly();
            if (assembly != null)
            {
                return assembly.GetType(className);
            }
            return null;
        }

        public override object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode)
        {
            var assembly = GetAssembly();
            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(resourceName);
            }
            return null;
        }

        public IAssemblyMetadata Import { get; private set; }
    }
}

