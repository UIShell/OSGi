namespace UIShell.OSGi.Loader
{
    using System.Reflection;
    using Dependency.Metadata;

    internal interface IAssemblyResolvingProvider : IRuntimeService
    {
        Assembly ResolveAssembly(AssemblyName assebmlyName);
        Assembly ResolveAssembly(IAssemblyMetadata metadata, AssemblyName assebmlyName);
        void Start();
        void Stop();
    }
}

