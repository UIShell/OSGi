namespace UIShell.OSGi.Dependency
{
    internal enum ResolverErrorType
    {
        MissingImportNamespace,
        MissingRequireBundle,
        MissingFragmentHost,
        FragmentConflict,
        SingletonSelection
    }
}

