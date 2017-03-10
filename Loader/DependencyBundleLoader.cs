namespace UIShell.OSGi.Loader
{
    using System;
    using OSGi;
    using Core.Bundle;
    using Dependency.Metadata;

    internal class DependencyBundleLoader : AbstractRuntimeLoader, IRuntimeLoader, IDependencyBundleLoader
    {
        public DependencyBundleLoader(IDependentBundleConstraint requireConstraintMetadata, IBundleLoader ownerLoader) : base(ownerLoader)
        {
            if (requireConstraintMetadata == null)
            {
                throw new ArgumentNullException();
            }
            Dependency = requireConstraintMetadata;
        }

        private IBundle FindDepdentBundle()
        {
            if (Dependency.DependentMetadata == null)
            {
                return null;
            }
            return base.Owner.Framework.Bundles.Find(delegate (IBundle bundle) {
                AbstractBundle bundle2 = bundle as AbstractBundle;
                return (bundle2 != null) && (bundle2.Metadata == Dependency.DependentMetadata);
            });
        }

        public override Type LoadClass(string className) =>
            FindDepdentBundle().LoadClass(className);

        public override object LoadResource(string resourceName, ResourceLoadMode resourceLoadMode)
        {
            Equals(resourceLoadMode, ResourceLoadMode.ClassSpace);
            return FindDepdentBundle().LoadResource(resourceName, resourceLoadMode);
        }

        public IDependentBundleConstraint Dependency { get; set; }
    }
}

