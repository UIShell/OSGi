namespace UIShell.OSGi.Persistent
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Utility;

    internal class PersistentHelper
    {
        public static object Load(string file, Type type)
        {
            try
            {
                var input = File.OpenRead(file);
                var obj2 = new XmlSerializer(type).Deserialize(new XmlTextReader(input));
                input.Close();
                return obj2;
            }
            catch (Exception ex)
            {
                FileLogUtility.Error(string.Format(Messages.FailedToLoadPersistence, file, ex.Message));
                FileLogUtility.Error(ex);
                try
                {
                    File.Move(file, file + ".failed");
                }
                catch
                {
                }
                return null;
            }
        }

        public static void Save<T>(string file, T obj)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var stream = new FileStream(file, FileMode.Create);
                serializer.Serialize(stream, obj);
                stream.Close();
            }
            catch (Exception ex)
            {
                FileLogUtility.Error(string.Format(Messages.FailedToSavePersistence, file, ex.Message));
                FileLogUtility.Error(ex);
            }
        }
    }
}

