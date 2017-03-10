namespace UIShell.OSGi.Console
{
    using System;
    using System.Collections.ObjectModel;
    using Core;

    internal class CommandContext : ICommandContext
    {
        public CommandContext(IFramework framework)
        {
            if (framework == null)
            {
                throw new ArgumentNullException();
            }
            Framework = framework;
        }

        public IFramework Framework { get; private set; }

        public bool Handled { get; set; }

        public string Message { get; set; }

        public ReadOnlyCollection<string> Words { get; internal set; }
    }
}

