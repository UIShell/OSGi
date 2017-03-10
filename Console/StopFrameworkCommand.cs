namespace UIShell.OSGi.Console
{
    internal class StopFrameworkCommand : ICommand
    {
        public const string CMD_FMT = "stop";

        public void Run(string cmd, ICommandContext context)
        {
            if (cmd.Equals(CMD_FMT))
            {
                context.Handled = true;
                context.Framework.Stop();
            }
        }
    }
}

