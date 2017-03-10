namespace UIShell.OSGi.Persistent
{
    using System;
    using OSGi;
    using Utility;

    [Serializable]
    public class BundlePersistentData : IPersistent
    {
        public BundlePersistentData(IBundle bundle)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            State = bundle.State;
        }

        public object Load(string file)
        {
            var data = PersistentHelper.Load(file, GetType()) as BundlePersistentData;
            if (data != null)
            {
                State = data.State;
            }
            return data;
        }

        public void Save(string file)
        {
            PersistentHelper.Save(file, this);
        }

        public BundleState State { get; set; }
    }
}

