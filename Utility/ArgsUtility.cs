namespace UIShell.OSGi.Utility
{
    using System;

    internal sealed class ArgsUtility
    {
        private static int? _initialBundleStartLevel;
        private static bool _parsedArgs = false;
        private static int? _startLevel;
        private const string InitialStartLevelLead = "-initial";
        private const string StartLevelLead = "-level";

        private ArgsUtility()
        {
        }

        private static void ParseArgs(object arguments)
        {
            if (!_parsedArgs)
            {
                if (arguments == null)
                {
                    arguments = Environment.GetCommandLineArgs();
                }

                for (var i = 0; i < ((string [])arguments).Length; i++)
                {
                    if (arguments[i].CompareTo("-initial") == 0)
                    {
                        if (++i >= arguments.Length)
                        {
                            throw new Exception("Invalid args,usage:[-initial 5] [-level 2]");
                        }
                        _initialBundleStartLevel = new int?(Convert.ToInt32((string) arguments[i]));
                    }
                    else if (arguments[i].CompareTo("-level") == 0)
                    {
                        if (++i >= arguments.Length)
                        {
                            throw new Exception("Invalid args,usage:[-initial 5] [-level 2]");
                        }
                        _startLevel = new int?(Convert.ToInt32((string) arguments[i]));
                    }
                }
                _parsedArgs = true;
            }
            return;
        }

        public static string[] GetCommandLineArgs =>
            Environment.GetCommandLineArgs();

        public static int? InitialBundleStartLevel
        {
            get
            {
                ParseArgs(null);
                return _initialBundleStartLevel;
            }
        }

        public static int? StartLevel
        {
            get
            {
                ParseArgs(null);
                return _startLevel;
            }
        }
    }
}

