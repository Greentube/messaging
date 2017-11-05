using System;
using System.IO;

namespace Serialization.Protobuf
{
    public class ProtobufSerializer : ISerializer
    {
        public byte[] Serialize<T>(T @object)
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, @object);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return ProtoBuf.Serializer.Deserialize(type, stream);
            }
        }
    }
}
