namespace UIShell.OSGi.Logging.Util
{
    using System;
    using System.Diagnostics;

    internal static class Clock
    {
        private static FuncDelegate<DateTime> nowProvider;
        private static FuncDelegate<DateTime> utcNowProvider;

        static Clock()
        {
            Reset();
        }

        private static DateTime MakeLocal(DateTime dt)
        {
            switch (dt.Kind)
            {
                case DateTimeKind.Unspecified:
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                    return dt;

                case DateTimeKind.Utc:
                    dt = dt.ToLocalTime();
                    return dt;
            }
            return dt;
        }

        private static DateTime MakeUtc(DateTime dt)
        {
            switch (dt.Kind)
            {
                case DateTimeKind.Unspecified:
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    return dt;

                case DateTimeKind.Utc:
                    return dt;

                case DateTimeKind.Local:
                    dt = dt.ToUniversalTime();
                    return dt;
            }
            return dt;
        }

        [DebuggerNonUserCode]
        internal static void Reset()
        {
            SetNowProvider(() => DateTime.Now, false);
            SetUtcNowProvider(() => DateTime.UtcNow, false);
        }

        [DebuggerNonUserCode]
        internal static void SetNow(DateTime now)
        {
            now = MakeLocal(now);
            SetNowProvider(() => now, false);
        }

        [DebuggerNonUserCode]
        internal static void SetNowProvider(FuncDelegate<DateTime> nowProvider)
        {
            if (nowProvider == null)
            {
                throw new ArgumentNullException("nowProvider");
            }
            SetNowProvider(nowProvider, true);
        }

        private static void SetNowProvider(FuncDelegate<DateTime> provider, bool wrap)
        {
            FuncDelegate<DateTime> delegate2 = null;
            if (wrap)
            {
                if (delegate2 == null)
                {
                    delegate2 = () => MakeLocal(provider());
                }
                nowProvider = delegate2;
            }
            else
            {
                nowProvider = provider;
            }
        }

        [DebuggerNonUserCode]
        internal static void SetUtcNow(DateTime now)
        {
            now = MakeUtc(now);
            SetUtcNowProvider(() => now, false);
        }

        [DebuggerNonUserCode]
        internal static void SetUtcNowProvider(FuncDelegate<DateTime> utcNowProvider)
        {
            if (utcNowProvider == null)
            {
                throw new ArgumentNullException("utcNowProvider");
            }
            SetUtcNowProvider(utcNowProvider, true);
        }

        private static void SetUtcNowProvider(FuncDelegate<DateTime> provider, bool wrap)
        {
            FuncDelegate<DateTime> delegate2 = null;
            if (wrap)
            {
                if (delegate2 == null)
                {
                    delegate2 = () => MakeUtc(provider());
                }
                utcNowProvider = delegate2;
            }
            else
            {
                utcNowProvider = provider;
            }
        }

        [DebuggerNonUserCode]
        internal static DateTime Now =>
            nowProvider();

        [DebuggerNonUserCode]
        internal static DateTime UtcNow =>
            utcNowProvider();
    }
}

