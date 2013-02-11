using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace org.theGecko.Utilities
{
    public static class ClassExtensions
    {
        /// <summary>
        /// Deep Clone a class using binary serialization (requires [Serializable] attribute)
        /// </summary>
        /// <typeparam name="T">Type to clone</typeparam>
        /// <param name="instance">Original instance to clone</param>
        /// <returns>Clone of instance</returns>
        public static T BinaryClone<T>(this T instance) where T : class//, ISerializable
        {
            using (Stream stream = new MemoryStream())
            {
                IFormatter serializer = new BinaryFormatter();
                serializer.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Deep Clone a class using xml serialization (requires parameterless constructor)
        /// </summary>
        /// <typeparam name="T">Type to clone</typeparam>
        /// <param name="instance">Original instance to clone</param>
        /// <returns>Clone of instance</returns>
        public static T XmlClone<T>(this T instance) where T : class, new()
        {
            using (Stream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }
    }
}