namespace UIShell.OSGi.Dependency.Resolver
{
    internal interface IAssemblyMetadataNode : IMetadataNode, IReferencable
    {
        System.Reflection.AssemblyName AssemblyName { get; set; }
    }
}

