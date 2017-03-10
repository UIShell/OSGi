namespace UIShell.OSGi.Core.Bundle
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Configuration.BundleManifest;
    using Dependency.Metadata;
    using Loader;
    using OSGi;
    using Utility;

    [DebuggerDisplay("Version:{Version}", Name="Bundle:{SymbolicName}")]
    internal abstract class AbstractBundle : IDisposable, IBundle
    {
        private IBundleMetadata _metadata;
        private BundleState _state;
        private AutoResetEvent _stateChangingAutoResetEvent;
        private object _stateChangingLock;
        private Thread _stateChangingThread;

        public AbstractBundle(BundleData bundleData, Framework framework)
        {
            if (framework == null)
            {
                throw new ArgumentNullException();
            }
            _stateChangingLock = new object();
            _stateChangingAutoResetEvent = new AutoResetEvent(false);
            BundleData = bundleData;
            Framework = framework;
            _state = BundleState.Installed;
            if (bundleData != null)
            {
                SymbolicName = bundleData.SymbolicName;
            }
            var proxy = new BundleLoaderProxy(this) {
                Framework = framework
            };
            BundleLoaderProxy = proxy;
            BundleType = string.IsNullOrEmpty(bundleData.HostBundleSymbolicName) ? BundleType.Host : BundleType.Fragment;
        }

        protected virtual void BeginStateChange()
        {
            lock (_stateChangingLock)
            {
                string str;
                for (bool flag = false; _stateChangingThread != null; flag = true)
                {
                    if (flag || (_stateChangingThread == Thread.CurrentThread))
                    {
                        goto Label_0041;
                    }
                    _stateChangingAutoResetEvent.WaitOne(0x1388, false);
                }
                goto Label_0065;
            Label_0041:
                str = string.Format(Messages.BundleStateChangeLockFailed, SymbolicName, Version);
                FileLogUtility.Error(str);
                throw new BundleException(str);
            Label_0065:
                _stateChangingThread = Thread.CurrentThread;
            }
        }

        protected virtual void CheckValidState()
        {
            if (State == BundleState.Uninstalled)
            {
                throw new IllegalStateException();
            }
        }

        protected virtual void CompleteStateChange()
        {
            lock (_stateChangingLock)
            {
                if (_stateChangingThread != null)
                {
                    _stateChangingThread = null;
                }
                _stateChangingAutoResetEvent.Set();
            }
        }

        protected virtual IBundleContext CreateBundleContext() => 
            new BundleContext(this);

        public abstract BundleLoader CreateBundleLoader();
        public virtual void Dispose()
        {
            _stateChangingAutoResetEvent = null;
            _stateChangingThread = null;
            _stateChangingLock = null;
            GC.SuppressFinalize(this);
        }

        private void DoLifecycleAction(Action action, string actionName)
        {
            CheckValidState();
            BeginStateChange();
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                action();
            }
            finally
            {
                stopwatch.Stop();
                FileLogUtility.Verbose(string.Format(Messages.BundleActionTimeCounter, new object[] { stopwatch.ElapsedMilliseconds, actionName, SymbolicName, Version }));
                CompleteStateChange();
            }
        }

        protected abstract void DoStart(BundleStartOptions option);
        protected abstract void DoStop(BundleStopOptions option);
        protected abstract void DoUninstall();
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is IBundle))
            {
                return base.Equals(obj);
            }
            IBundle bundle = obj as IBundle;
            return (Equals(SymbolicName, bundle.SymbolicName) && Equals(Version, bundle.Version));
        }

        public virtual int GetBunldeStartLevel()
        {
            if (!BundleData.StartLevel.HasValue)
            {
                return Framework.StartLevelManager.InitialBundleStartLevel;
            }
            return BundleData.StartLevel.Value;
        }

        public override int GetHashCode()
        {
            int num = string.IsNullOrEmpty(SymbolicName) ? 0 : SymbolicName.GetHashCode();
            int num2 = (Version == null) ? 0 : Version.GetHashCode();
            return (num ^ num2);
        }

        public virtual Type LoadClass(string className)
        {
            Type type;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                type = BundleLoaderProxy.LoadClass(className);
            }
            finally
            {
                stopwatch.Stop();
                FileLogUtility.Verbose(string.Format(Messages.LoadClassTimeCounter, new object[] { stopwatch.ElapsedMilliseconds, className, SymbolicName, Version }));
            }
            return type;
        }

        public virtual object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            object obj2;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                obj2 = BundleLoaderProxy.LoadResource(resourceName, loadMode);
            }
            finally
            {
                stopwatch.Stop();
                FileLogUtility.Verbose(string.Format(Messages.LoadResourceTimeCounter, new object[] { stopwatch.ElapsedMilliseconds, resourceName, SymbolicName, Version }));
            }
            return obj2;
        }

        public virtual void Start(BundleStartOptions option)
        {
            if ((Framework.StartLevelManager.StartLevel < GetBunldeStartLevel()) && (option == BundleStartOptions.Transient))
            {
                throw new BundleException(string.Format(Messages.NotStartedSinceStartLevel, SymbolicName, Version));
            }
            FileLogUtility.Debug(string.Format(Messages.BundleStarting, SymbolicName, Version));
            DoLifecycleAction(() => DoStart(option), Messages.StartAction);
            FileLogUtility.Debug(string.Format(Messages.BundleInState, SymbolicName, Version, State));
        }

        public virtual void Stop(BundleStopOptions option)
        {
            FileLogUtility.Debug(string.Format(Messages.BundleStopping, SymbolicName, Version));
            DoLifecycleAction(() => DoStop(option), Messages.StopAction);
            FileLogUtility.Debug(string.Format(Messages.BundleInState, SymbolicName, Version, State));
        }

        public override string ToString()
        {
            if (BundleData == null)
            {
                return string.Format("Name:{0},BundleData:{null}", IsActive.ToString());
            }
            return $"Name:{BundleData.SymbolicName},IsActive:{IsActive.ToString()},State:{State.ToString()}";
        }

        public virtual void Uninstall()
        {
            if (IsActive)
            {
                try
                {
                    Stop(BundleStopOptions.Transient);
                }
                catch (Exception exception)
                {
                    FileLogUtility.Error(string.Format(Messages.ExceptionOccursWhenUninstalling, SymbolicName, Version));
                    FileLogUtility.Error(exception);
                }
            }
            FileLogUtility.Debug(string.Format(Messages.BundleUninstalling, SymbolicName, Version));
            DoLifecycleAction(() => DoUninstall(), Messages.UninstallAction);
            FileLogUtility.Debug(string.Format(Messages.BundleInState, SymbolicName, Version, State));
        }

        public ActivatorPolicy ActivatorPolicy =>
            BundleData.Activator?.Policy;

        internal Type ActivatorType { get; set; }

        public BundleData BundleData { get; private set; }

        public long BundleID { get; internal set; }

        public BundleLoaderProxy BundleLoaderProxy { get; private set; }

        public BundleType BundleType { get; private set; }

        public IBundleContext Context { get; internal set; }

        public Framework Framework { get; private set; }

        public bool IsActive =>
            (_state == BundleState.Active);

        internal bool IsFragment =>
            !string.IsNullOrEmpty(BundleData.HostBundleSymbolicName);

        internal bool IsResolved =>
            Metadata.IsResolved;

        public string Location { get; internal set; }

        internal IBundleMetadata Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = Framework.GetBundleMetadata(BundleID);
                }
                return _metadata;
            }
        }

        public string Name
        {
            get
            {
                if (BundleData != null)
                {
                    return BundleData.Name;
                }
                return string.Empty;
            }
        }

        public int StartLevel =>
            GetBunldeStartLevel();

        public BundleState State
        {
            get
            {
                return _state;
            }
            internal set
            {
                if (_state != value)
                {
                    BundleState previousState = _state;
                    _state = value;
                    Framework.EventManager.DispatchBundleEvent(this, new BundleStateChangedEventArgs(previousState, _state, this));
                }
            }
        }

        public string SymbolicName { get; private set; }

        public Version Version
        {
            get
            {
                if (BundleData != null)
                {
                    return BundleData.Version;
                }
                return null;
            }
        }
    }
}

