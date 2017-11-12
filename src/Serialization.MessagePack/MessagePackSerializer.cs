using System;
using MessagePack;
using NeueccMessagePackSerializer = MessagePack.MessagePackSerializer;

namespace Serialization.MessagePack
{
    /// <inheritdoc />
    public class MessagePackSerializer : ISerializer
    {
        private readonly MessagePackOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="MessagePackSerializer"/>
        /// </summary>
        /// <param name="options"></param>
        public MessagePackSerializer(MessagePackOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));

            return _options.UseLz4Compression
                ? LZ4MessagePackSerializer.Serialize(@object, _options.FormatterResolver)
                : NeueccMessagePackSerializer.Serialize(@object, _options.FormatterResolver);
        }

        /// <inheritdoc />
        public object Deserialize(Type type, byte[] bytes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            return _options.UseLz4Compression
                ? LZ4MessagePackSerializer.NonGeneric.Deserialize(type, bytes, _options.FormatterResolver)
                : NeueccMessagePackSerializer.NonGeneric.Deserialize(type, bytes, _options.FormatterResolver);
        }
    }
}