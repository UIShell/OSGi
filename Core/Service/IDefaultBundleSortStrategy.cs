namespace UIShell.OSGi.Core.Service
{
    using System.Collections.Generic;
    using Bundle;

    internal interface IDefaultBundleSortStrategy
    {
        void Sort(List<HostBundle> bundles);
    }
}

