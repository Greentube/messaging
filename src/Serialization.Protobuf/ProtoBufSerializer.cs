using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Serialization.Protobuf
{
    public class ProtoBufSerializer : ISerializer
    {
        private readonly ProtoBufOptions _options;

        public ProtoBufSerializer(IOptions<ProtoBufOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public byte[] Serialize<T>(T @object)
        {
            using (var stream = new MemoryStream())
            {
                _options.RuntimeTypeModel.Serialize(stream, @object);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return (T)_options.RuntimeTypeModel.Deserialize(stream, null, typeof(T));
            }
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return _options.RuntimeTypeModel.Deserialize(stream, null, type);
            }
        }
    }
}
