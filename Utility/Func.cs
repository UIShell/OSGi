namespace UIShell.OSGi.Utility
{
    internal delegate TResult Func<TResult>();

    internal delegate TResult Func<T, TResult>(T arg);

    internal delegate TResult Func<T, T2, TResult>(T arg);
}

