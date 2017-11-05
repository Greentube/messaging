using System;
using System.Threading;
using System.Threading.Tasks;
using Serialization;

namespace Messaging
{
    public class DispatchingRawMessageHandler : IRawMessageHandler
    {
        private readonly IMessageHandlerInvoker _messageHandlerInvoker;
        private readonly IMessageTypeTopicMap _typeTopicMap;
        private readonly ISerializer _serializer;

        public DispatchingRawMessageHandler(
            IMessageTypeTopicMap typeTopicMap,
            ISerializer serializer,
            IMessageHandlerInvoker messageHandlerInvoker)
        {
            _messageHandlerInvoker = messageHandlerInvoker ?? throw new ArgumentNullException(nameof(messageHandlerInvoker));
            _typeTopicMap = typeTopicMap ?? throw new ArgumentNullException(nameof(typeTopicMap));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task Handle(string topic, byte[] message, CancellationToken token)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageType = _typeTopicMap.Get(topic);
            if (messageType == null)
            {
                throw new InvalidOperationException(
                    $"Topic '{topic}' has no message type registered with: {_typeTopicMap.GetType()}.");
            }

            var deserialized = _serializer.Deserialize(messageType, message);
            if (deserialized == null)
            {
                throw new InvalidOperationException(
                    $"Serializer {_serializer.GetType()} returned null for the {message.Length}-byte message of type {messageType}.");
            }

            return _messageHandlerInvoker.Invoke(deserialized, token);
        }
    }
}