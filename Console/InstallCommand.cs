namespace UIShell.OSGi.Console
{
    using Core.Service;
    using Utility;

    internal class InstallCommand : ICommand
    {
        private const string CMD_FMT = "install";

        public void Run(string cmd, ICommandContext context)
        {
            if (context.Words[0].Equals(CMD_FMT))
            {
                if (context.Words.Count == 2)
                {
                    context.Framework.ServiceContainer.GetFirstOrDefaultService<IBundleManager>().InstallBundle(context.Words[1]);
                }
                else
                {
                    context.Message = Messages.InstallCmdUsage;
                }
                context.Handled = true;
            }
        }
    }
}

