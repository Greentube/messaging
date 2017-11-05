using System;
using System.Text;
using Newtonsoft.Json;

namespace Serialization.Json
{
    public class JsonConvertSerializer : ISerializer
    {
        public byte[] Serialize<T>(T @object)
        {
            var @string = JsonConvert.SerializeObject(@object);
            return Encoding.UTF8.GetBytes(@string);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var @string = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(@string);
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            var @string = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject(@string, type);
        }
    }
}