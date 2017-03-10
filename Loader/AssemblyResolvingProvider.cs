namespace UIShell.OSGi.Loader
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Configuration.BundleManifest;
    using Core;
    using Core.Adaptor;
    using Core.Bundle;
    using Dependency;
    using Dependency.Metadata;
    using Dependency.Resolver;
    using OSGi;
    using Utility;

    internal class AssemblyResolvingProvider : IDisposable, IRuntimeService, IAssemblyResolvingProvider
    {
        private ReaderWriterLock _lock = new ReaderWriterLock();

        public AssemblyResolvingProvider(IFrameworkAdaptor frameworkAdaptor, IFramework framework)
        {
            this.Framework = framework;
            this.FrameworkAdaptor = frameworkAdaptor;
            this.OSGiAssembly = base.GetType().Assembly;
            this.OSGiAssemblyName = this.OSGiAssembly.GetName();
            try
            {
                this.OSGiWebExtensionAssembly = Assembly.Load("UIShell.OSGi.WebExtension");
                if (this.OSGiWebExtensionAssembly != null)
                {
                    this.OSGiWebExtensionAssemblyName = this.OSGiWebExtensionAssembly.GetName();
                }
            }
            catch
            {
            }
            this.Cache = new Dictionary<string, AssemblyEntry>();
            this.Resolver = frameworkAdaptor.State.Resolver;
            this.Resolver.BundleUnresolved += new EventHandler<BundleMetadataEventArgs>(Resolver_BundleUnresolved);
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs e)
        {
            FileLogUtility.Debug(string.Format(Messages.AssemblyLoadedMessage, e.LoadedAssembly.FullName));
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly2;
            if (UseDefaultClrLoaderBehavior.IsStopClrExtensionLoader)
            {
                if (UseDefaultClrLoaderBehavior.AssemblyResolve != null)
                {
                    return UseDefaultClrLoaderBehavior.AssemblyResolve(sender, args);
                }
                return null;
            }
            AssemblyName assemblyName = null;
            try
            {
                assemblyName = new AssemblyName(args.Name);
                Assembly assembly = this.TryToLoadCoreAssemblies(assemblyName);
                if (assembly == null)
                {
                    if (assemblyName != null)
                    {
                        assembly = this.ResolveAssembly(assemblyName);
                    }
                    if (assembly == null)
                    {
                        FileLogUtility.Error($"Failed to load assembly '{args.Name}'.");
                    }
                }
                return assembly;
            }
            catch (Exception exception)
            {
                FileLogUtility.Error($"Failed to load assembly '{args.Name}'. The exception will be shown as below.");
                FileLogUtility.Error(exception);
                assembly2 = null;
            }
            return assembly2;
        }

        private AssemblyEntry FindCacheByPath(string path)
        {
            AssemblyEntry entry3;
            using (ReaderWriterLockHelper.CreateReaderLock(this._lock))
            {
                AssemblyEntry entry = null;
                using (Dictionary<string, AssemblyEntry>.ValueCollection.Enumerator enumerator = this.Cache.Values.GetEnumerator())
                {
                    AssemblyEntry current;
                    while (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        if ((current != null) && (current.AssemblyPath == path))
                        {
                            goto Label_0046;
                        }
                    }
                    goto Label_0058;
                Label_0046:
                    entry = current;
                }
            Label_0058:
                entry3 = entry;
            }
            return entry3;
        }

        private string GetAssemblyFullPath(IAssemblyMetadata metadata)
        {
            BundleData member = this.FrameworkAdaptor.InstalledBundles.Find(a => a.SymbolicName == metadata.Owner.SymbolicName);
            AssertUtility.NotNull(member);
            return BundleUtility.FindAssemblyFullPath(member.Path, metadata.Path, true);
        }

        public List<Assembly> LoadBundleAssembly(string bundleSymbolicName)
        {
            List<Assembly> result = new List<Assembly>();
            Interface2 interface2 = this.Resolver.ResolvedNodes.Find(node => node.SymbolicName == bundleSymbolicName);
            if (interface2 != null)
            {
                interface2.SharedAssemblyNodes.ForEach(delegate (IAssemblyMetadataNode node) {
                    IAssemblyMetadata metadata = (IAssemblyMetadata) node.Metadata;
                    if (!metadata.IsDuplicatedWithGlobalAssembly)
                    {
                        result.Add(this.ResolveAssembly(metadata, null, true));
                    }
                });
                interface2.PrivateAssemblyNodes.ForEach(delegate (IAssemblyMetadataNode node) {
                    IAssemblyMetadata metadata = (IAssemblyMetadata) node.Metadata;
                    if (!metadata.IsDuplicatedWithGlobalAssembly)
                    {
                        result.Add(this.ResolveAssembly(metadata, null, true));
                    }
                });
            }
            return result;
        }

        public Assembly ResolveAssembly(AssemblyName assebmlyFullName)
        {
            Func<AssemblyName, bool> stopWhen = null;
            using (ReaderWriterLockHelper.CreateReaderLock(this._lock))
            {
                AssemblyEntry entry;
                if (this.Cache.TryGetValue(assebmlyFullName.FullName, out entry))
                {
                    return entry.Assembly;
                }
            }
            IAssemblyMetadataNode node = this.Resolver.ResolvedAssemblyMetadataNodes.Find(delegate (IAssemblyMetadataNode resolvedAssemblyNode) {
                IAssemblyMetadata metadata = resolvedAssemblyNode.Metadata as IAssemblyMetadata;
                AssertUtility.NotNull(metadata);
                if (assebmlyFullName.Version == null)
                {
                    if (!metadata.MultipleVersions)
                    {
                        return assebmlyFullName.Name.Equals(metadata.AssemblyName.Name);
                    }
                }
                else if (StringUtility.IsAssemblyFullNameMatch(metadata.AssemblyName, assebmlyFullName, true))
                {
                    return true;
                }
                return false;
            });
            if (node == null)
            {
                return null;
            }
            if (stopWhen == null)
            {
                stopWhen = assembly => StringUtility.IsAssemblyFullNameMatch(assembly, assebmlyFullName, true);
            }
            return this.ResolveAssembly((IAssemblyMetadata) node.Metadata, stopWhen, false);
        }

        public Assembly ResolveAssembly(IAssemblyMetadata metadata, AssemblyName assemblyName)
        {
            using (ReaderWriterLockHelper.CreateReaderLock(this._lock))
            {
                AssemblyEntry entry;
                if ((assemblyName != null) && this.Cache.TryGetValue(assemblyName.FullName, out entry))
                {
                    return entry.Assembly;
                }
            }
            return this.ResolveAssembly(metadata, assembly => StringUtility.IsAssemblyFullNameMatch(assembly, assemblyName, true), true);
        }

        private Assembly ResolveAssembly(IAssemblyMetadata metadata, Func<AssemblyName, bool> stopWhen, bool tryCache)
        {
            string assemblyFullPath = this.GetAssemblyFullPath(metadata);
            AssemblyEntry entry = this.FindCacheByPath(assemblyFullPath);
            if (entry == null)
            {
                TryStartBundle(this.Framework.GetBundleBySymbolicName(metadata.Owner.SymbolicName));
                entry = this.FindCacheByPath(assemblyFullPath);
                if (entry == null)
                {
                    entry = new AssemblyEntry {
                        Assembly = Assembly.LoadFile(assemblyFullPath),
                        ProvidedBy = metadata,
                        AssemblyPath = assemblyFullPath
                    };
                    using (ReaderWriterLockHelper.CreateWriterLock(this._lock))
                    {
                        if (!this.Cache.ContainsKey(entry.Assembly.FullName))
                        {
                            this.Cache.Add(entry.Assembly.FullName, entry);
                        }
                    }
                }
            }
            if ((stopWhen != null) && stopWhen(entry.Assembly.GetName()))
            {
                return entry.Assembly;
            }
            return entry.Assembly;
        }

        private void Resolver_BundleUnresolved(object sender, BundleMetadataEventArgs e)
        {
            using (ReaderWriterLockHelper.CreateWriterLock(this._lock))
            {
                Dictionary<string, AssemblyEntry> dictionary = new Dictionary<string, AssemblyEntry>(this.Cache);
                foreach (KeyValuePair<string, AssemblyEntry> pair in dictionary)
                {
                    if (pair.Value.ProvidedBy.Owner == e.Metadata)
                    {
                        this.Cache.Remove(pair.Key);
                    }
                }
            }
        }

        public void Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(this.CurrentDomain_AssemblyLoad);
        }

        public void Stop()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler(this.CurrentDomain_AssemblyLoad);
            using (ReaderWriterLockHelper.CreateWriterLock(this._lock))
            {
                this.Cache.Clear();
            }
        }

        void IDisposable.Dispose()
        {
            this.Stop();
        }

        private static void TryStartBundle(IBundle ownerBundle)
        {
            if ((ownerBundle != null) && !(ownerBundle is FragmentBundle))
            {
                if ((ownerBundle.State == BundleState.Installed) || (ownerBundle.State == BundleState.Resolved))
                {
                    ownerBundle.Start(BundleStartOptions.General);
                }
                if (ownerBundle.State == BundleState.Starting)
                {
                    IHostBundle bundle = ownerBundle as IHostBundle;
                    if (bundle != null)
                    {
                        bundle.ActivateForStarting();
                    }
                }
            }
        }

        private Assembly TryToLoadCoreAssemblies(AssemblyName assemblyName)
        {
            if (assemblyName.Name.Equals(this.OSGiAssemblyName.Name))
            {
                return this.OSGiAssembly;
            }
            if ((this.OSGiWebExtensionAssemblyName != null) && assemblyName.Name.Equals(this.OSGiWebExtensionAssemblyName.Name))
            {
                return this.OSGiWebExtensionAssembly;
            }
            return null;
        }

        public Dictionary<string, AssemblyEntry> Cache { get; private set; }

        public IFramework Framework { get; private set; }

        public IFrameworkAdaptor FrameworkAdaptor { get; private set; }

        private Assembly OSGiAssembly { get; set; }

        private AssemblyName OSGiAssemblyName { get; set; }

        private Assembly OSGiWebExtensionAssembly { get; set; }

        private AssemblyName OSGiWebExtensionAssemblyName { get; set; }

        public IResolver Resolver { get; private set; }
    }
}

