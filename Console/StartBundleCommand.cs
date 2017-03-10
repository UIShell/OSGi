namespace UIShell.OSGi.Console
{
    using OSGi;
    using Utility;

    internal class StartBundleCommand : ICommand
    {
        public void Run(string cmd, ICommandContext context)
        {
            if ((context.Words.Count == 2) && (context.Words[0].CompareTo("start") == 0))
            {
                IBundle bundleBySymbolicName = context.Framework.GetBundleBySymbolicName(context.Words[1]);
                if (bundleBySymbolicName != null)
                {
                    bundleBySymbolicName.Start(BundleStartOptions.General);
                }
                else
                {
                    context.Message = string.Format(Messages.BundleNotExist, context.Words[1]);
                }
                context.Handled = true;
            }
        }
    }
}

