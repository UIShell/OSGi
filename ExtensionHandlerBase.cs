namespace UIShell.OSGi
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Collection;

    public abstract class ExtensionHandlerBase<T> : IDisposable
    {
        private bool _disposed;

        protected IBundleContext BundleContext { get; private set; }

        protected ThreadSafeDictionary<Extension, List<T>> ExtensionDatas { get; private set; }

        protected string ExtensionPoint { get; private set; }

        public ExtensionHandlerBase(IBundleContext bundleContext, string extensionPoint)
        {
            BundleContext = bundleContext;
            ExtensionPoint = extensionPoint;
            ExtensionDatas = new ThreadSafeDictionary<Extension, List<T>>();
            HandleExtensions();
            BundleContext.ExtensionChanged += new EventHandler<ExtensionEventArgs>(BundleContextExtensionChanged);
        }

        protected virtual void AddExtensionData(Extension extension, T extensionData)
        {
            using (var locker = ExtensionDatas.Lock())
            {
                if (!locker.ContainsKey(extension))
                {
                    locker[extension] = new List<T>();
                }
                locker[extension].Add(extensionData);
            }
        }

        protected virtual void BundleContextExtensionChanged(object sender, ExtensionEventArgs e)
        {
            if (e.ExtensionPoint.Equals(ExtensionPoint))
            {
                if (e.Action == CollectionChangedAction.Add)
                {
                    foreach (T local in ConvertExtensionToExtensionDatas(e.Extension))
                    {
                        AddExtensionData(e.Extension, local);
                    }
                }
                else if (e.Action == CollectionChangedAction.Remove)
                {
                    RemoveExtensionDatas(e.Extension);
                }
                else
                {
                    ClearExtensionDatas();
                }
            }
        }

        protected virtual void ClearExtensionDatas()
        {
            ExtensionDatas.ForEach(item => item.Value.Clear());
            ExtensionDatas.Clear();
        }

        protected virtual List<T> ConvertExtensionToExtensionDatas(Extension extension)
        {
            var list = new List<T>();
            T item = default(T);
            foreach (var node in extension.Data)
            {
                if (!(node is XmlComment))
                {
                    item = ConvertXmlNodeToExtensionData(extension, node);
                    if (item != null)
                    {
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                        }
                        else
                        {
                            IgnoreDuplicatedExtensionData(item);
                        }
                    }
                }
            }
            return list;
        }

        protected abstract T ConvertXmlNodeToExtensionData(Extension extension, XmlNode node);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    BundleContext.ExtensionChanged -= new EventHandler<ExtensionEventArgs>(BundleContextExtensionChanged);
                    ClearExtensionDatas();
                }
                _disposed = true;
            }
        }

        ~ExtensionHandlerBase()
        {
            Dispose(false);
        }

        public List<T> GetExtensionDatas(IBundle bundle)
        {
            var result = new List<T>();
            ExtensionDatas.ForEach(delegate (KeyValuePair<Extension, List<T>> item) {
                if (item.Key.Owner.Equals(bundle))
                {
                    result.AddRange(item.Value);
                }
            });
            return result;
        }

        public T GetFirstOrDefaultExtensionData(IBundle bundle)
        {
            T data = default(T);
            ExtensionDatas.ForEach(delegate (KeyValuePair<Extension, List<T>> item) {
                if (item.Key.Owner.Equals(bundle) && (item.Value.Count > 0))
                {
                    data = item.Value[0];
                }
            });
            return data;
        }

        protected string GetXmlNodeAttributeValue(XmlNode node, string attributeName)
        {
            var str = string.Empty;
            if ((node.Attributes[attributeName] != null) && !string.IsNullOrEmpty(node.Attributes[attributeName].Value.Trim()))
            {
                str = node.Attributes[attributeName].Value.Trim();
            }
            return str;
        }

        protected virtual void HandleExtensions()
        {
            foreach (var extension in BundleContext.GetExtensions(ExtensionPoint))
            {
                foreach (T local in ConvertExtensionToExtensionDatas(extension))
                {
                    AddExtensionData(extension, local);
                }
            }
        }

        protected abstract void IgnoreDuplicatedExtensionData(T extensionData);
        protected virtual void RemoveExtensionDatas(Extension extension)
        {
            ExtensionDatas.Remove(extension);
        }
    }
}

