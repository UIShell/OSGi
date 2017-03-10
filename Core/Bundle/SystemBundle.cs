namespace UIShell.OSGi.Core.Bundle
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Loader;
    using Utility;

    internal class SystemBundle : AbstractBundle
    {
        public SystemBundle(Framework framework) : base(data, framework)
        {
            BundleData data = new BundleData {
                SymbolicName = "UIShell.OSGi.SystemBundle",
                Version = FrameworkConstants.DEFAULT_VERSION,
                Name = "SystemBundle"
            };
        }

        public override BundleLoader CreateBundleLoader()
        {
            throw new NotSupportedException();
        }

        protected override void DoStart(BundleStartOptions option)
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                if (((base.State != BundleState.Active) && (base.State != BundleState.Starting)) && (base.State != BundleState.Stopping))
                {
                    stopwatch.Start();
                    base.State = BundleState.Starting;
                    if (base.Context == null)
                    {
                        base.Context = this.CreateBundleContext();
                    }
                    base.Framework.StartLevelManager.ChangeStartLevel(base.Framework.StartLevelManager.StartLevel);
                    base.State = BundleState.Active;
                }
            }
            finally
            {
                stopwatch.Stop();
                FileLogUtility.Verbose(string.Format(Messages.StartSystemBundleTimeCounter, stopwatch.ElapsedMilliseconds));
            }
        }

        protected override void DoStop(BundleStopOptions option)
        {
            int startLevel;
            if (base.State == BundleState.Active)
            {
                base.State = BundleState.Stopping;
                startLevel = base.Framework.StartLevelManager.StartLevel;
                List<IBundle> list = base.Framework.Bundles.FindAll(bundle => bundle.GetBunldeStartLevel() <= startLevel);
                list.Sort((bundle1, bundle2) => bundle2.GetBunldeStartLevel().CompareTo(bundle1.GetBunldeStartLevel()));
                int num = 0;
                while (true)
                {
                    if (num >= list.Count)
                    {
                        break;
                    }
                    HostBundle bundle = list[num] as HostBundle;
                    if (bundle != null)
                    {
                        try
                        {
                            list[num].Stop(BundleStopOptions.Transient);
                            base.Framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Stopped, list[num]));
                        }
                        catch (Exception exception)
                        {
                            FileLogUtility.Error(string.Format(Messages.StopBundleFailed, bundle.SymbolicName, bundle.Version));
                            FileLogUtility.Error(exception);
                            base.Framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Error, list[num], exception));
                        }
                    }
                    num++;
                }
                base.State = BundleState.Resolved;
            }
        }

        protected override void DoUninstall()
        {
            throw new BundleException("System bundle can not be uninstalled.");
        }

        public override int GetBunldeStartLevel() => 
            0;

        public override Type LoadClass(string className) => 
            Type.GetType(className);

        public override object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            throw new NotImplementedException();
        }

        public virtual void Start()
        {
            base.Start(BundleStartOptions.Transient);
        }

        internal void Stop()
        {
            this.DoStop(BundleStopOptions.Transient);
        }

        public override void Stop(BundleStopOptions option)
        {
            if (base.State == BundleState.Active)
            {
                new Thread(delegate {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        stopwatch.Start();
                        base.Framework.Stop();
                    }
                    finally
                    {
                        stopwatch.Stop();
                        FileLogUtility.Verbose(string.Format(Messages.StopSystemBundleTimeCounter, stopwatch.ElapsedMilliseconds));
                    }
                }).Start();
            }
        }

        public override string ToString() => 
            (base.SymbolicName + ", " + base.Version.ToString());
    }
}

