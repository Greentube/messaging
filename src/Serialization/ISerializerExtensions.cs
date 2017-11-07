using System;

namespace Serialization
{
    public static class SerializerExtensions
    {
        /// <summary>
        /// Deserializes the byte array into an instance of T
        /// </summary>
        /// <param name="bytes">The data to deserialize</param>
        /// <typeparam name="T">The type to instantiate</typeparam>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(this ISerializer serializer, byte[] bytes)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return (T)serializer.Deserialize(typeof(T), bytes);
        }
    }
}