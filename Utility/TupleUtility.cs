namespace UIShell.OSGi.Utility
{
    public class TupleUtility
    {
        public static Tuple<T1, T2> CreateTuple<T1, T2>(T1 t1, T2 t2) => 
            new Tuple<T1, T2>(t1, t2);

        public static Tuple<T1, T2, T3> CreateTuple<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => 
            new Tuple<T1, T2, T3>(t1, t2, t3);

        public static Tuple<T1, T2, T3, T4> CreateTuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) => 
            new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
    }
}

