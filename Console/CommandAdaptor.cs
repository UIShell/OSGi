namespace UIShell.OSGi.Console
{
    using System;
    using System.Diagnostics;

    internal class CommandAdaptor : ICommandAdaptor
    {
        private ICommand _cmd;
        private Type _commandType;

        private CommandAdaptor(Type commandType)
        {
            _commandType = commandType;
        }

        public static CommandAdaptor CreateAdaptor<T>() where T: ICommand, new() => 
            new CommandAdaptor(typeof(T));

        public ICommand Command
        {
            [DebuggerStepThrough]
            get
            {
                if (_cmd == null)
                {
                    _cmd = (ICommand) Activator.CreateInstance(_commandType);
                }
                return _cmd;
            }
        }
    }
}

