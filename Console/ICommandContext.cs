namespace UIShell.OSGi.Console
{
    using System.Collections.ObjectModel;
    using Core;

    public interface ICommandContext
    {
        IFramework Framework { get; }

        bool Handled { get; set; }

        string Message { get; set; }

        ReadOnlyCollection<string> Words { get; }
    }
}

