using System;
using System.IO;
using MsXmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace Greentube.Serialization.Xml
{
    /// <summary>
    /// XML Serialization
    /// </summary>
    /// <inheritdoc />
    public class XmlSerializer : ISerializer
    {
        private readonly XmlOptions _options;

        public XmlSerializer(XmlOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));

            using (var stream = new MemoryStream())
            {
                var serializer = Create(typeof(T));
                serializer.Serialize(stream, @object);
                return stream.ToArray();
            }
        }

        /// <inheritdoc />
        public object Deserialize(Type type, byte[] bytes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            using (var stream = new MemoryStream(bytes))
            {
                var serializer = Create(type);
                return serializer.Deserialize(stream);
            }
        }

        private MsXmlSerializer Create(Type type)
        {
            if (_options.Factory != null)
            {
                return _options.Factory(type);
            }

            return _options.DefaultNamespace != null
                ? new MsXmlSerializer(type, _options.DefaultNamespace)
                : new MsXmlSerializer(type);
        }
    }
}
