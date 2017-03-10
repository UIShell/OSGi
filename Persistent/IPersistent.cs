namespace UIShell.OSGi.Persistent
{
    public interface IPersistent
    {
        object Load(string file);
        void Save(string file);
    }
}

