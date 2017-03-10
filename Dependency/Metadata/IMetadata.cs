namespace UIShell.OSGi.Dependency.Metadata
{
    internal interface IMetadata
    {
        IBundleMetadata Owner { get; set; }

        System.Version Version { get; set; }
    }
}

