namespace UIShell.OSGi.Console
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Core;
    using Core.Command;

    internal class CommandRepository : GInterface0
    {
        private IFramework _framework;

        public CommandRepository(IFramework framework)
        {
            if (framework == null)
            {
                throw new ArgumentNullException();
            }
            this._framework = framework;
            this.Commands = new List<ICommandAdaptor>();
            this.NamedCommands = new Dictionary<string, ICommandAdaptor>();
        }

        public bool Run(string cmd)
        {
            ICommandAdaptor adaptor;
            if (string.IsNullOrEmpty(cmd))
            {
                throw new ArgumentNullException();
            }
            CommandContext context = new CommandContext(this._framework) {
                Words = this.SplitCmdWords(cmd)
            };
            if (this.NamedCommands.TryGetValue(cmd, out adaptor))
            {
                adaptor.Command.Run(cmd, context);
            }
            else
            {
                foreach (ICommandAdaptor adaptor2 in this.Commands)
                {
                    adaptor2.Command.Run(cmd, context);
                    if (context.Handled)
                    {
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(context.Message))
            {
                Console.WriteLine(context.Message);
            }
            return context.Handled;
        }

        private ReadOnlyCollection<string> SplitCmdWords(string cmd)
        {
            var strArray = cmd.Split(new char[] { ' ' });
            var list = new List<string>(strArray.Length);
            for (var i = 0; i < strArray.Length; i++)
            {
                if (!strArray[i].Equals(" "))
                {
                    list.Add(strArray[i]);
                }
            }
            return list.AsReadOnly();
        }

        public IList<ICommandAdaptor> Commands { get; private set; }

        public Dictionary<string, ICommandAdaptor> NamedCommands { get; private set; }
    }
}

