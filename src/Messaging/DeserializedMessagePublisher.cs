using System;
using System.Threading.Tasks;
using Serialization;

namespace Messaging
{
    /// <summary>
    /// Publishes a deserialized message via raw publisher
    /// </summary>
    public class DeserializedMessagePublisher : IMessagePublisher
    {
        private readonly IMessageTypeTopicMap _typeTopicMap;
        private readonly IRawMessagePublisher _rawMessagePublisher;
        private readonly ISerializer _serializer;

        public DeserializedMessagePublisher(
            IMessageTypeTopicMap typeTopicMap,
            ISerializer serializer,
            IRawMessagePublisher rawMessagePublisher)
        {
            _typeTopicMap = typeTopicMap ?? throw new ArgumentNullException(nameof(typeTopicMap));
            _rawMessagePublisher = rawMessagePublisher ?? throw new ArgumentNullException(nameof(rawMessagePublisher));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task Publish<TMessage>(TMessage message)
        {
            var topic = _typeTopicMap.Get(message.GetType());
            if (topic == null)
            {
                throw new InvalidOperationException(
                    $"Message type {message.GetType()} is not registered with: {_typeTopicMap.GetType()}.");
            }

            var serialized = _serializer.Serialize(message);
            if (serialized == null)
            {
                throw new InvalidOperationException(
                    $"Serializer {serialized.GetType()} returned null for message of type {message.GetType()}.");
            }

            return _rawMessagePublisher.Publish(topic, serialized);
        }
    }
}