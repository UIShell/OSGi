namespace UIShell.OSGi.Dependency.Resolver.Impl
{
    using System;
    using System.Collections.Generic;
    using UIShell.OSGi;
    using UIShell.OSGi.Dependency;
    using UIShell.OSGi.Dependency.Metadata;
    using UIShell.OSGi.Dependency.Resolver;

    internal class HostConstraintNode : ConstraintNode, IResolvable, IConstraintNode, Interface0
    {
        public HostConstraintNode(IResolver resolver, Interface2 owner, IHostConstraint constraint) : base(resolver, owner, constraint)
        {
            this.BundleSymbolicName = constraint.BundleSymbolicName;
            this.BundleVersion = this.BundleVersion;
        }

        public override bool Resolve()
        {
            throw new NotImplementedException();
        }

        public override bool Unresolve()
        {
            throw new NotImplementedException();
        }

        public string BundleSymbolicName { get; private set; }

        public VersionRange BundleVersion { get; private set; }

        protected override List<IMetadataNode> ResolveNodeSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

