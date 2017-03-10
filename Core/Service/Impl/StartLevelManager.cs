namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using OSGi;
    using Core;
    using Bundle;
    using Service;
    using Utility;

    internal class StartLevelManager : GInterface1
    {
        private IFramework _framework;
        private int? _initialBundleStartLevel;
        private int? _startLevel;
        private const int DefaultInitialBundleStartLevel = 50;
        private const int DefaultStartLevel = 100;

        public StartLevelManager(IFramework framework)
        {
            _framework = framework;
        }

        public void ChangeBundleStartLevel(IBundle bundle, int startLevel)
        {
            if (bundle.GetBunldeStartLevel() <= startLevel)
            {
                bundle.Start(BundleStartOptions.General);
            }
            else
            {
                bundle.Stop(BundleStopOptions.General);
            }
        }

        public void ChangeStartLevel(int startLevel)
        {
            if (this._framework == null)
            {
                throw new InvalidOperationException();
            }
            List<IBundle> list = this._framework.Bundles.FindAll(bundle => bundle.GetBunldeStartLevel() <= startLevel);
            list.Sort((bundle1, bundle2) => bundle1.GetBunldeStartLevel().CompareTo(bundle2.GetBunldeStartLevel()));
            for (int i = 0; i < list.Count; i++)
            {
                HostBundle bundle = list[i] as HostBundle;
                if (bundle != null)
                {
                    try
                    {
                        DefaultBundleState? bundleStartState = bundle.GetBundleStartState();
                        if (!bundleStartState.HasValue)
                        {
                            if (this._framework.Options.DefaultBundleState == DefaultBundleState.Active)
                            {
                                list[i].Start(BundleStartOptions.Transient);
                            }
                        }
                        else if (((DefaultBundleState) bundleStartState.Value) == DefaultBundleState.Active)
                        {
                            list[i].Start(BundleStartOptions.General);
                        }
                    }
                    catch (Exception exception)
                    {
                        FileLogUtility.Error(string.Format(Messages.StartBundleFailed, bundle.SymbolicName, bundle.Version));
                        FileLogUtility.Error(exception);
                        this._framework.EventManager.DispatchFrameworkEvent(this, new FrameworkEventArgs(FrameworkEventType.Error, list[i], exception));
                    }
                }
            }
        }

        [DefaultValue(50)]
        public int InitialBundleStartLevel
        {
            get
            {
                if (this._initialBundleStartLevel.HasValue)
                {
                    return this._initialBundleStartLevel.Value;
                }
                if (ArgsUtility.InitialBundleStartLevel.HasValue)
                {
                    this._initialBundleStartLevel = ArgsUtility.InitialBundleStartLevel;
                    return this._initialBundleStartLevel.Value;
                }
                return 50;
            }
            set
            {
                this._initialBundleStartLevel = new int?(value);
            }
        }

        public int StartLevel
        {
            get
            {
                if (this._startLevel.HasValue)
                {
                    return this._startLevel.Value;
                }
                if (ArgsUtility.StartLevel.HasValue)
                {
                    this._startLevel = ArgsUtility.StartLevel;
                    return this._startLevel.Value;
                }
                return 100;
            }
            set
            {
                this._startLevel = new int?(value);
            }
        }
    }
}

