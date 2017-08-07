using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Arachnode.Utilities
{
    public static class Serialization
    {
        private static object _lock = new object();

        public static void SerializeObject(string filename, object objectToSerialize)
        {
            lock (_lock)
            {
                //using (Stream stream = File.Open(filename, FileMode.Create))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();

                //    bFormatter.Serialize(stream, objectToSerialize);
                //}
                try
                {
                    if (Delimon.Win32.IO.File.Exists(filename))
                    {
                        Delimon.Win32.IO.File.Delete(filename);
                    }

                    Delimon.Win32.IO.File.WriteAllText(filename, JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented));
                    //TODO: The Delimon IO is ~12% slower than the standard .NET File.WriteAllText operation...
                    //File.WriteAllText(filename, JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented));
                }
                catch
                {
                }
            }
        }

        public static object DeserializeObject(string filename, Type type)
        {
            object objectToDeserialize = null;

            lock (_lock)
            {
                //using (Stream stream = File.Open(filename, FileMode.Open))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();

                //    objectToDeserialize = (object)bFormatter.Deserialize(stream);
                //}
                try
                {
                    return JsonConvert.DeserializeObject(Delimon.Win32.IO.File.ReadAllText(filename), type);
                    //return JsonConvert.DeserializeObject(File.ReadAllText(filename), type);
                }
                catch
                {
                }
            }

            return objectToDeserialize;
        }
    }
}
