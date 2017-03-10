namespace UIShell.OSGi.Dependency.Metadata.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;
    using UIShell.OSGi.Core;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Loader;
    using UIShell.OSGi.Utility;

    internal class MetadataBuilder : IMetadataBuilder
    {
        private IFramework _framework;

        public MetadataBuilder(IFramework framework)
        {
            this._framework = framework;
        }

        private List<IAssemblyMetadata> BuildAssemblyPathConstraintMetadata(AssemblyData assemblyData, IBundleMetadata bundleMetadata)
        {
            List<IAssemblyMetadata> list = new List<IAssemblyMetadata>(assemblyData.PathArray.Length);
            foreach (string str in assemblyData.PathArray)
            {
                AssemblyMetadata item = new AssemblyMetadata {
                    Path = str,
                    Owner = bundleMetadata,
                    MultipleVersions = assemblyData.MultipleVersions
                };
                list.Add(item);
            }
            return list;
        }

        public IBundleMetadata BuildBundleMetadata(BundleData bundleData, long bundleID)
        {
            AssertUtility.ArgumentNotNull(bundleData, "bundleData");
            IBundleMetadata bundleMetadata = null;
            if (!string.IsNullOrEmpty(bundleData.HostBundleSymbolicName))
            {
                FragmentBundleMetadata metadata = new FragmentBundleMetadata();
                bundleMetadata = metadata;
                HostConstraint constraint = new HostConstraint {
                    BundleSymbolicName = bundleData.HostBundleSymbolicName,
                    BundleVersion = bundleData.HostBundleVersion
                };
                metadata.HostConstraint = constraint;
            }
            else
            {
                bundleMetadata = new HostBundleMetadata();
            }
            bundleMetadata.BundleID = bundleID;
            bundleMetadata.SymbolicName = bundleData.SymbolicName;
            if (bundleData.Version != null)
            {
                bundleMetadata.Version = bundleData.Version;
            }
            else
            {
                bundleMetadata.Version = FrameworkConstants.DEFAULT_VERSION;
            }
            bundleMetadata.Location = bundleData.Path;
            bundleData.Runtime.Assemblies.ForEach(delegate (AssemblyData assemblyData) {
                List<IAssemblyMetadata> assemblyMetadata = this.BuildAssemblyPathConstraintMetadata(assemblyData, bundleMetadata);
                this.ResolveAndValidateAssembly(bundleData.Path, assemblyMetadata);
                if (assemblyData.Share)
                {
                    bundleMetadata.SharedAssemblies.AddRange(assemblyMetadata);
                }
                else
                {
                    bundleMetadata.PrivateAssemblies.AddRange(assemblyMetadata);
                }
            });
            bundleData.Runtime.Dependencies.ForEach(delegate (DependencyData a) {
                if (!string.IsNullOrEmpty(a.AssemblyName))
                {
                    DependentAssemblyConstraint item = new DependentAssemblyConstraint {
                        AssemblyName = a.AssemblyName,
                        BundleSymbolicName = a.BundleSymbolicName,
                        AssemblyVersion = a.AssemblyVersion,
                        BundleVersion = a.BundleVersion,
                        Owner = bundleMetadata,
                        Resolution = a.Resolution
                    };
                    bundleMetadata.DependentAssemblies.Add(item);
                }
                else
                {
                    bundleMetadata.DependentBundles.Add(this.BuildDependencyConstraintMetadata(a, bundleMetadata));
                }
            });
            return bundleMetadata;
        }

        private IDependentBundleConstraint BuildDependencyConstraintMetadata(DependencyData requireBundleData, IBundleMetadata bundleMetadata) => 
            new DependentBundleConstraint { 
                BundleSymbolicName = requireBundleData.BundleSymbolicName,
                BundleVersion = requireBundleData.BundleVersion,
                Owner = bundleMetadata
            };

        private void ResolveAndValidateAssembly(string bundlePath, List<IAssemblyMetadata> assemblyMetadata)
        {
            assemblyMetadata.RemoveAll(delegate (IAssemblyMetadata item) {
                try
                {
                    AssemblyName name3;
                    string assemblyFile = BundleUtility.FindAssemblyFullPath(bundlePath, item.Path, true);
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                    if (assemblyName == null)
                    {
                        assemblyName = new AssemblyName {
                            Name = Path.GetFileNameWithoutExtension(assemblyFile),
                            Version = FrameworkConstants.DEFAULT_VERSION,
                            CultureInfo = FrameworkConstants.DEFAULT_CULTURE
                        };
                    }
                    item.Path = assemblyFile;
                    item.Version = assemblyName.Version;
                    item.AssemblyName = assemblyName;
                    if (this.TryToReplaceWithGlobalAssembly(item, out name3))
                    {
                        FileLogUtility.Warn(string.Format(Messages.LocalAssemblyReplacedByGlobal, new object[] { item.AssemblyName, item.Owner.SymbolicName, item.Owner.Version, name3 }));
                        item.IsDuplicatedWithGlobalAssembly = true;
                        item.AssemblyName = name3;
                    }
                    return false;
                }
                catch (Exception exception)
                {
                    item.Owner.AssembliesFailedToLoad.Add(item);
                    FileLogUtility.Error(string.Format(Messages.BundleAssemblyLoadFailed, new object[] { item.Path, item.Owner.SymbolicName, item.Owner.Version, exception.Message }));
                    return true;
                }
            });
        }

        private bool TryToReplaceWithGlobalAssembly(IAssemblyMetadata assemblyMetadata, out AssemblyName globalAssemblyName)
        {
            bool flag;
            globalAssemblyName = null;
            UseDefaultClrLoaderBehavior behavior = new UseDefaultClrLoaderBehavior();
            try
            {
                Assembly assembly = Assembly.ReflectionOnlyLoad(assemblyMetadata.AssemblyName.Name);
                AssemblyName name = assembly?.GetName();
                if ((assembly != null) && VersionUtility.Compatible(assemblyMetadata.AssemblyName.Version, name.Version))
                {
                    globalAssemblyName = name;
                    return true;
                }
                flag = false;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (behavior != null)
                {
                    behavior.Dispose();
                }
            }
            return flag;
        }
    }
}

