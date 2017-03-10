namespace UIShell.OSGi.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OSGi;
    using Dependency.Metadata;
    using Dependency.Resolver;

    internal sealed class BundleUtility
    {
        private BundleUtility()
        {
        }

        public static void BindFragmentMetadatas(Interface1 host, List<IFragmentBundleMetadata> framentMetadatas)
        {
            host.Fragments.Clear();
            framentMetadatas.ForEach(delegate (IFragmentBundleMetadata item) {
                if ((host.SymbolicName == item.HostConstraint.BundleSymbolicName) && ((item.HostConstraint.BundleVersion == null) || item.HostConstraint.BundleVersion.IsIncluded(host.Version)))
                {
                    host.Fragments.Add(item);
                    item.HostConstraint.DependentMetadata = host;
                }
            });
        }

        internal static void BuildFragments(IHostBundleMetadataNode hostNode)
        {
            AssertUtility.ArgumentNotNull(hostNode, "hostNode");
            Interface1 metadata = hostNode.Metadata as Interface1;
            AssertUtility.ArgumentNotNull(metadata, "metadata");
            if ((metadata.Fragments != null) && (metadata.Fragments.Count != 0))
            {
                hostNode.DetachAllFragments();
                metadata.Fragments.ForEach(a => hostNode.AttachFragment((IFragmentBundleMetadataNode)hostNode.ConstraintResolver.UnResolverNodes.Find(node => node.Metadata == a)));
            }
        }

        public static string FindAssemblyFullPath(string bundlePath, string searchFile)
        {
            if (Path.IsPathRooted(searchFile))
            {
                return Path.GetFullPath(searchFile);
            }
            string path = Path.Combine(bundlePath, searchFile);
            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
            path = FindAssemblyFullPathInSubFolders(bundlePath, searchFile, "bin".Split(new char[] { ';' }));
            if (!string.IsNullOrEmpty(path))
            {
                return Path.GetFullPath(path);
            }
            path = FindAssemblyFullPathInSubFolders(bundlePath, searchFile, FrameworkConstants.ASSEMBLY_SEARCH_FOLDERS_FOR_NONDEBUG.Split(new char[] { ';' }));
            if (!string.IsNullOrEmpty(path))
            {
                return Path.GetFullPath(path);
            }
            return string.Empty;
        }

        public static string FindAssemblyFullPath(string bundlePath, string searchFile, bool throwIfNotFound)
        {
            string str = FindAssemblyFullPath(bundlePath, searchFile);
            if (throwIfNotFound && (string.IsNullOrEmpty(str) || !File.Exists(str)))
            {
                throw new Exception($"Can not find file: {searchFile} under {bundlePath}");
            }
            return str;
        }

        private static string FindAssemblyFullPathInSubFolders(string bundlePath, string searchFile, IEnumerable<string> subFolders)
        {
            string path = string.Empty;
            foreach (string str2 in subFolders)
            {
                path = Path.Combine(Path.Combine(bundlePath, str2), searchFile);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return string.Empty;
        }

        public static List<T> GetMetadatas<T>(List<IBundleMetadata> metadatas) where T: IBundleMetadata => 
            metadatas.FindAll(metadata => metadata is T).ConvertAll<T>(bundleMetadata => ((T)bundleMetadata));
    }
}

