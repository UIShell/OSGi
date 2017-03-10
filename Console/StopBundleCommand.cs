namespace UIShell.OSGi.Console
{
    using OSGi;
    using Utility;

    internal class StopBundleCommand : ICommand
    {
        public void Run(string cmd, ICommandContext context)
        {
            if ((context.Words.Count == 2) && (context.Words[0].CompareTo("stop") == 0))
            {
                IBundle bundleBySymbolicName = context.Framework.GetBundleBySymbolicName(context.Words[1]);
                if (bundleBySymbolicName != null)
                {
                    bundleBySymbolicName.Stop(BundleStopOptions.General);
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

