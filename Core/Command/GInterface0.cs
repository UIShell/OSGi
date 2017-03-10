namespace UIShell.OSGi.Core.Command
{
    using System.Collections.Generic;

    public interface GInterface0
    {
        bool Run(string cmd);

        IList<ICommandAdaptor> Commands { get; }

        Dictionary<string, ICommandAdaptor> NamedCommands { get; }
    }
}

