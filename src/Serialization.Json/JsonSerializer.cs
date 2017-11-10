using System;
using Newtonsoft.Json;

namespace Serialization.Json
{
    /// <inheritdoc />
    public class JsonSerializer : ISerializer
    {
        private readonly JsonOptions _options;

        public JsonSerializer(JsonOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));

            var @string = JsonConvert.SerializeObject(@object);
            return _options.Encoding.GetBytes(@string);
        }

        /// <inheritdoc />
        public object Deserialize(Type type, byte[] bytes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var @string = _options.Encoding.GetString(bytes);
            return JsonConvert.DeserializeObject(@string, type);
        }
    }
}