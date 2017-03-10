namespace UIShell.OSGi
{
    using System;

    [Serializable]
    public enum BundleState
    {
        Installed,
        Resolved,
        Starting,
        Active,
        Stopping,
        Uninstalled
    }
}

