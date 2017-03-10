namespace UIShell.OSGi
{
    public class BundleStateChangedEventArgs : BundleEventArgs
    {
        internal BundleStateChangedEventArgs(BundleState previousState, BundleState currentState, IBundle bundle)
            : base(bundle)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public override string ToString()
        {
            return $"Bundle:{Bundle},PreviousState:{PreviousState},CurrentState:{CurrentState}";
        }

        public BundleState CurrentState { get; private set; }

        public BundleState PreviousState { get; private set; }
    }
}

