using System;
using System.IO;

namespace Serialization.Protobuf
{
    /// <inheritdoc />
    public class ProtoBufSerializer : ISerializer
    {
        private readonly ProtoBufOptions _options;

        public ProtoBufSerializer(ProtoBufOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T @object)
        {
            using (var stream = new MemoryStream())
            {
                _options.RuntimeTypeModel.Serialize(stream, @object);
                return stream.ToArray();
            }
        }

        /// <inheritdoc />
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
