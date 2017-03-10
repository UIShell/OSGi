namespace UIShell.OSGi.Loader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Threading;
    using OSGi;
    using Core;
    using Utility;

    internal class BundleLoader : AbstractRuntimeLoader, IRuntimeLoader, IBundleLoader
    {
        private ReaderWriterLock _cacheLock = new ReaderWriterLock();
        private OrderedDictionary _classAssemblyNameCache = new OrderedDictionary();

        public BundleLoader(IBundle bundle)
        {
            if (bundle == null)
            {
                throw new ArgumentNullException();
            }
            Owner = this;
            Bundle = bundle;
            AssemblyPathLoaders = new List<AssemblyLoader>();
            DependencyAssemblyLoaders = new List<DependencyAssemblyLoader>();
            DependencyBundleLoaders = new List<DependencyBundleLoader>();
        }

        protected void CacheLoadedType(string className, Type type)
        {
            if (BundleRuntime.Instance.EnableBundleClassLoaderCache)
            {
                using (ReaderWriterLockHelper.CreateWriterLock(_cacheLock))
                {
                    if (!_classAssemblyNameCache.Contains(className))
                    {
                        if (_classAssemblyNameCache.Count >= BundleRuntime.Instance.BundleClassLoaderCacheSize)
                        {
                            FileLogUtility.Inform(string.Format(Messages.RemoveCachedTypeForSize, Bundle.SymbolicName, Bundle.Version, BundleRuntime.Instance.BundleClassLoaderCacheSize));
                            _classAssemblyNameCache.RemoveAt(0);
                        }
                        _classAssemblyNameCache.Add(className, type);
                    }
                }
            }
        }

        private Type GetGlobalType(string className)
        {
            if (className.IndexOf(",") < 0)
            {
                Type type2;
                string str = className;
                using (new UseDefaultClrLoaderBehavior())
                {
                    int num;
                    Type type;
                    while ((num = str.LastIndexOf(".")) > 0)
                    {
                        str = str.Substring(0, num);
                        type = Type.GetType($"{className}, {str}");
                        if (type != null)
                        {
                            goto Label_0049;
                        }
                    }
                    goto Label_005B;
                Label_0049:
                    type2 = type;
                }
                return type2;
            }
        Label_005B:
            return null;
        }

        public override Type LoadClass(string className)
        {
            Type rt = null;
            if (!TryGetCachedType(className, out rt))
            {
                rt = Type.GetType(className);
                if (rt != null)
                {
                    return rt;
                }
                if (BundleRuntime.Instance.EnableGlobalAssemblyFeature)
                {
                    rt = GetGlobalType(className);
                    if (rt != null)
                    {
                        CacheLoadedType(className, rt);
                        return rt;
                    }
                }
                if ((!AssemblyPathLoaders.Exists(item => (rt = item.LoadClass(className)) != null) && !DependencyAssemblyLoaders.Exists(item => (rt = item.LoadClass(className)) != null)) && !DependencyBundleLoaders.Exists(item => (rt = item.LoadClass(className)) != null))
                {
                    return rt;
                }
                CacheLoadedType(className, rt);
            }
            return rt;
        }

        private object LoadFromDependencyAssemblies(string resourceName)
        {
            object result = null;
            DependencyAssemblyLoaders.Exists(item => (result = item.LoadResource(resourceName, ResourceLoadMode.Local)) != null);
            return result;
        }

        private object LoadFromDependentBundles(string resourceName)
        {
            object result = null;
            DependencyBundleLoaders.Exists(item => (result = item.LoadResource(resourceName, ResourceLoadMode.Local)) != null);
            return result;
        }

        private object LoadFromLocalAssemblies(string resourceName)
        {
            object result = null;
            AssemblyPathLoaders.Exists(item => (result = item.LoadResource(resourceName, ResourceLoadMode.Local)) != null);
            return result;
        }

        public override object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode)
        {
            object obj2 = null;
            switch (resourceLoadMode)
            {
                case ResourceLoadMode.Local:
                case ResourceLoadMode.LocalAndFragment:
                    return LoadFromLocalAssemblies(resourceName);

                case ResourceLoadMode.ClassSpace:
                    obj2 = LoadFromLocalAssemblies(resourceName);
                    if (obj2 == null)
                    {
                        obj2 = LoadFromDependencyAssemblies(resourceName);
                        if (obj2 == null)
                        {
                            obj2 = LoadFromDependentBundles(resourceName);
                            if (obj2 != null)
                            {
                                return obj2;
                            }
                        }
                        return obj2;
                    }
                    return obj2;
            }
            return obj2;
        }

        protected bool TryGetCachedType(string className, out Type type)
        {
            type = null;
            if (BundleRuntime.Instance.EnableBundleClassLoaderCache)
            {
                using (ReaderWriterLockHelper.CreateReaderLock(_cacheLock))
                {
                    if (_classAssemblyNameCache.Contains(className))
                    {
                        type = _classAssemblyNameCache[className] as Type;
                        return true;
                    }
                }
            }
            return false;
        }

        public List<AssemblyLoader> AssemblyPathLoaders { get; private set; }

        public IBundle Bundle { get; private set; }

        public List<DependencyAssemblyLoader> DependencyAssemblyLoaders { get; private set; }

        public List<DependencyBundleLoader> DependencyBundleLoaders { get; private set; }

        public IFramework Framework { get; set; }
    }
}

