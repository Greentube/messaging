using System;
using System.IO;

namespace Greentube.Serialization.ProtoBuf
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
            using (var stream = new MemoryStream(bytes))
            {
                return _options.RuntimeTypeModel.Deserialize(stream, null, type);
            }
        }
    }
}
