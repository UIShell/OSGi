namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UIShell.OSGi;
    using UIShell.OSGi.Configuration.BundleManifest;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;

    [DebuggerDisplay("DependentBundleConstraintNode-BundleSymbolicName:{BundleSymbolicName},IsResolved:{IsResolved},IsResolvable:{IsResolvable}")]
    internal class DependentBundleConstraintNode : ConstraintNode, IResolvable, IConstraintNode, IDependentable, IDependentBundleConstraintNode
    {
        public DependentBundleConstraintNode(IResolver resolver, Interface2 owner, IDependentBundleConstraint constraint) : base(resolver, owner, constraint)
        {
            this.BundleSymbolicName = constraint.BundleSymbolicName;
            this.BundleVersion = constraint.BundleVersion;
            this.Resolution = constraint.Resolution;
        }

        public override string ToString() => 
            $"Owner bundle: {base.Owner}, dependent bundle name: {this.BundleSymbolicName}, version: {this.BundleVersion}";

        public string BundleSymbolicName { get; private set; }

        public VersionRange BundleVersion { get; private set; }

        public ResolutionType Resolution { get; private set; }

        protected override List<IMetadataNode> ResolveNodeSource
        {
            get
            {
                List<IMetadataNode> rt = new List<IMetadataNode>();
                base.ConstraintResolver.ResolvedNodes.ForEach(delegate (Interface2 resolvedNode) {
                    rt.Add(resolvedNode);
                });
                base.ConstraintResolver.UnResolverNodes.FindAll(unresolvedNode => unresolvedNode.IsResolvable).ForEach(delegate (Interface2 item) {
                    rt.Add(item);
                });
                return rt;
            }
        }
    }
}

