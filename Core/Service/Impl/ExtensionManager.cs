namespace UIShell.OSGi.Core.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Service;
    using Dependency.Metadata;
    using Utility;

    internal class ExtensionManager : IDisposable, IExtensionManager
    {
        private EventHandler<ExtensionEventArgs> _extensionChanged;
        private EventHandler<ExtensionPointEventArgs> _extensionPointChanged;
        private Framework _framework;
        private ReaderWriterLock _locker = new ReaderWriterLock();
        private object _syncRootForExtensionChanged = new object();
        private object _syncRootForExtensionPointChanged = new object();

        public event EventHandler<ExtensionEventArgs> ExtensionChanged
        {
            add
            {
                lock (this._syncRootForExtensionChanged)
                {
                    this._extensionChanged = (EventHandler<ExtensionEventArgs>) Delegate.Combine(this._extensionChanged, value);
                }
            }
            remove
            {
                lock (this._syncRootForExtensionChanged)
                {
                    this._extensionChanged = (EventHandler<ExtensionEventArgs>) Delegate.Remove(this._extensionChanged, value);
                }
            }
        }

        public event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged
        {
            add
            {
                lock (this._syncRootForExtensionPointChanged)
                {
                    this._extensionPointChanged = (EventHandler<ExtensionPointEventArgs>) Delegate.Combine(this._extensionPointChanged, value);
                }
            }
            remove
            {
                lock (this._syncRootForExtensionPointChanged)
                {
                    this._extensionPointChanged = (EventHandler<ExtensionPointEventArgs>) Delegate.Remove(this._extensionPointChanged, value);
                }
            }
        }

        public ExtensionManager(Framework framework)
        {
            AssertUtility.ArgumentNotNull(framework, "framework");
            this._framework = framework;
            this.ExtensionPoints = new List<ExtensionPoint>();
            this.Extensions = new Dictionary<string, List<Extension>>();
            framework.EventManager.AddBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(this.BundleEventHandler), true);
        }

        public void AddExtension(string extensionPoint, Extension extension)
        {
            using (ReaderWriterLockHelper.CreateWriterLock(this._locker))
            {
                if (!this.Extensions.ContainsKey(extensionPoint))
                {
                    this.Extensions[extensionPoint] = new List<Extension>();
                }
                this.Extensions[extensionPoint].Add(extension);
            }
            this.FireExtensionChanged(extensionPoint, CollectionChangedAction.Add, extension);
        }

        private void AddExtension(IBundle bundle, IEnumerable<ExtensionData> list)
        {
            foreach (ExtensionData data in list)
            {
                Extension extension = new Extension {
                    Owner = bundle
                };
                extension.Data.AddRange(data.ChildNodes);
                this.AddExtension(data.Point, extension);
            }
        }

        private void AddExtensionPoint(IBundle bundle, List<ExtensionPointData> list)
        {
            List<ExtensionPoint> list2 = new List<ExtensionPoint>();
            Dictionary<string, Extension> dictionary = new Dictionary<string, Extension>();
            using (ReaderWriterLockHelper.CreateWriterLock(this._locker))
            {
                foreach (ExtensionPointData data in list)
                {
                    ExtensionPoint item = new ExtensionPoint(this) {
                        Owner = bundle,
                        Point = data.Point,
                        Schema = data.Schema
                    };
                    this.ExtensionPoints.Add(item);
                    list2.Add(item);
                    if (data.ChildNodes.Count > 0)
                    {
                        Extension extension = new Extension {
                            Owner = bundle
                        };
                        extension.Data.AddRange(data.ChildNodes);
                        if (!this.Extensions.ContainsKey(data.Point))
                        {
                            this.Extensions[data.Point] = new List<Extension>();
                        }
                        this.Extensions[data.Point].Add(extension);
                        dictionary[data.Point] = extension;
                    }
                }
            }
            foreach (ExtensionPoint point3 in list2)
            {
                this.FireExtensionPointChanged(CollectionChangedAction.Add, point3);
            }
            foreach (KeyValuePair<string, Extension> pair in dictionary)
            {
                this.FireExtensionChanged(pair.Key, CollectionChangedAction.Add, pair.Value);
            }
        }

        private void BundleEventHandler(object sender, BundleStateChangedEventArgs e)
        {
            bool flag = e.CurrentState == BundleState.Starting;
            IBundleInstallerService firstOrDefaultService = this._framework.ServiceContainer.GetFirstOrDefaultService<IBundleInstallerService>();
            BundleData bundleDataByName = firstOrDefaultService.GetBundleDataByName(e.Bundle.SymbolicName);
            if (bundleDataByName != null)
            {
                if (flag)
                {
                    this.AddExtensionPoint(e.Bundle, bundleDataByName.ExtensionPoints);
                    this.AddExtension(e.Bundle, bundleDataByName.Extensions);
                    Interface1 bundleByID = this._framework.FrameworkAdaptor.State.GetBundleByID(e.Bundle.BundleID) as Interface1;
                    if (bundleByID != null)
                    {
                        BundleData data2 = null;
                        foreach (IFragmentBundleMetadata metadata in bundleByID.Fragments)
                        {
                            data2 = firstOrDefaultService.GetBundleDataByName(metadata.SymbolicName);
                            this.AddExtensionPoint(e.Bundle, data2.ExtensionPoints);
                            this.AddExtension(e.Bundle, data2.Extensions);
                        }
                    }
                }
                else if (e.CurrentState == BundleState.Stopping)
                {
                    this.ReleaseExtension(e.Bundle);
                }
            }
        }

        public void Dispose()
        {
            this.ExtensionPoints.Clear();
            this.Extensions.Clear();
            this._framework.EventManager.RemoveBundleEventListener(new EventHandler<BundleStateChangedEventArgs>(this.BundleEventHandler), true);
        }

        private void FireExtensionChanged(string extensionPoint, CollectionChangedAction action, Extension extension)
        {
            ExtensionEventArgs args = new ExtensionEventArgs(extensionPoint, this, action) {
                Extension = extension
            };
            this.OnExtensionChanged(args);
        }

        private void FireExtensionPointChanged(CollectionChangedAction action, ExtensionPoint extensionPoint)
        {
            ExtensionPointEventArgs args = new ExtensionPointEventArgs(this, action) {
                ExtensionPoint = extensionPoint
            };
            this.OnExtensionPointChanged(args);
        }

        private void FireExtensionsChanged(string extensionPoint, CollectionChangedAction action, List<Extension> extensions)
        {
            foreach (Extension extension in extensions)
            {
                this.FireExtensionChanged(extensionPoint, action, extension);
            }
        }

        public ExtensionPoint GetExtensionPoint(string extensionPoint)
        {
            extensionPoint = extensionPoint.Trim();
            using (ReaderWriterLockHelper.CreateReaderLock(this._locker))
            {
                foreach (ExtensionPoint point in this.ExtensionPoints)
                {
                    if (point.Point == extensionPoint)
                    {
                        return point;
                    }
                }
                if (this.GetExtensions(extensionPoint).Count > 0)
                {
                    return new ExtensionPoint(this) { Point = extensionPoint };
                }
            }
            return null;
        }

        public List<ExtensionPoint> GetExtensionPoints()
        {
            using (ReaderWriterLockHelper.CreateReaderLock(this._locker))
            {
                List<ExtensionPoint> list = new List<ExtensionPoint>();
                list.AddRange(this.ExtensionPoints);
                return list;
            }
        }

        public List<ExtensionPoint> GetExtensionPoints(IBundle bundle)
        {
            using (ReaderWriterLockHelper.CreateReaderLock(this._locker))
            {
                List<ExtensionPoint> list = new List<ExtensionPoint>();
                foreach (ExtensionPoint point in this.ExtensionPoints)
                {
                    if (point.Owner == bundle)
                    {
                        list.Add(point);
                    }
                }
                return list;
            }
        }

        public List<Extension> GetExtensions(string extensionPoint)
        {
            using (ReaderWriterLockHelper.CreateReaderLock(this._locker))
            {
                List<Extension> list2;
                List<Extension> list = new List<Extension>();
                if (this.Extensions.TryGetValue(extensionPoint, out list2))
                {
                    list.AddRange(list2);
                }
                return list;
            }
        }

        private void OnExtensionChanged(ExtensionEventArgs args)
        {
            if (this._extensionChanged != null)
            {
                this._extensionChanged(this, args);
            }
        }

        private void OnExtensionPointChanged(ExtensionPointEventArgs args)
        {
            if (this._extensionPointChanged != null)
            {
                this._extensionPointChanged(this, args);
            }
        }

        private void ReleaseExtension(IBundle bundle)
        {
            Func<Extension, bool> comparer = null;
            Func<ExtensionPoint, bool> func2 = null;
            Dictionary<string, List<Extension>> dictionary = new Dictionary<string, List<Extension>>();
            List<ExtensionPoint> list = new List<ExtensionPoint>();
            using (ReaderWriterLockHelper.CreateWriterLock(this._locker))
            {
                foreach (KeyValuePair<string, List<Extension>> pair in this.Extensions)
                {
                    if (comparer == null)
                    {
                        comparer = item => item.Owner == bundle;
                    }
                    dictionary[pair.Key] = ListUtility.RemoveAll<Extension>(pair.Value, comparer);
                }
                if (func2 == null)
                {
                    func2 = item => item.Owner == bundle;
                }
                list = ListUtility.RemoveAll<ExtensionPoint>(this.ExtensionPoints, func2);
            }
            foreach (KeyValuePair<string, List<Extension>> pair2 in dictionary)
            {
                this.FireExtensionsChanged(pair2.Key, CollectionChangedAction.Remove, pair2.Value);
            }
            foreach (ExtensionPoint point in list)
            {
                this.FireExtensionPointChanged(CollectionChangedAction.Remove, point);
            }
        }

        public void RemoveExtension(Extension extension)
        {
            string str = string.Empty;
            using (ReaderWriterLockHelper.CreateWriterLock(this._locker))
            {
                using (Dictionary<string, List<Extension>>.Enumerator enumerator = this.Extensions.GetEnumerator())
                {
                    KeyValuePair<string, List<Extension>> current;
                    while (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        if (current.Value.Remove(extension))
                        {
                            goto Label_0043;
                        }
                    }
                    goto Label_0067;
                Label_0043:
                    str = current.Key;
                }
            }
        Label_0067:
            if (!string.IsNullOrEmpty(str))
            {
                this.FireExtensionChanged(str, CollectionChangedAction.Remove, extension);
            }
        }

        public List<ExtensionPoint> ExtensionPoints { get; private set; }

        public Dictionary<string, List<Extension>> Extensions { get; private set; }
    }
}

