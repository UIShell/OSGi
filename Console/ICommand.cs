namespace UIShell.OSGi.Console
{
    public interface ICommand
    {
        void Run(string cmd, ICommandContext context);
    }
}

