using MessagePack;

namespace Serialization.MessagePack
{
    /// <summary>
    /// Options for MessagePack
    /// </summary>
    public class MessagePackOptions
    {
        /// <summary>
        /// Use the LZ4 compression implementation
        /// </summary>
        public bool UseLz4Compression { get; set; }
        /// <summary>
        /// The <see cref="IFormatterResolver"/> to use when Serializing and Deserializing
        /// </summary>
        public IFormatterResolver FormatterResolver { get; set; }
    }
}