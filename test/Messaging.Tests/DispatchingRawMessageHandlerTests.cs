using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Serialization;
using Xunit;
using static System.Threading.CancellationToken;

namespace Messaging.Tests
{
    public class DispatchingRawMessageHandlerTests
    {
        [Theory, AutoSubstituteData]
        public async Task Handle_DeserializedMessage_CallsInvoker(
            string topic,
            byte[] message,
            object deserializedObject,
            [Frozen] IMessageHandlerInvoker messageHandlerInvoker,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            [Frozen] ISerializer serializer,
            DispatchingRawMessageHandler sut)
        {
            // Arrange
            var messageType = GetType();
            messageTypeTopicMap.Get(topic).Returns(messageType);
            serializer.Deserialize(messageType, message).Returns(deserializedObject);

            // Act
            await sut.Handle(topic, message, None);

            // Assert
            await messageHandlerInvoker.Received().Invoke(deserializedObject, None);
        }

        [Theory, AutoSubstituteData]
        public async Task Handle_DeserializerReturnsNull_ThrowsInvalidOperation(
            string topic,
            byte[] message,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            [Frozen] ISerializer serializer,
            DispatchingRawMessageHandler sut)
        {
            var messageType = GetType();
            messageTypeTopicMap.Get(topic).Returns(messageType);
            serializer.Deserialize(messageType, message).ReturnsNull();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(topic, message, None));
            Assert.Equal($"Serializer {serializer.GetType()} returned null for the {message.Length}-byte message of type {messageType}.", ex.Message);
        }

        [Theory, AutoSubstituteData]
        public async Task Handle_TopicMapNotDefined_ThrowsInvalidOperation(
            string topic,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            DispatchingRawMessageHandler sut)
        {
            messageTypeTopicMap.Get(topic).ReturnsNull();
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(topic, new byte[0], None));
            Assert.Equal($"Topic '{topic}' has no message type registered with: {messageTypeTopicMap.GetType()}.", ex.Message);
        }

        [Theory, AutoSubstituteData]
        public async Task Handle_NullTopic_ThrowsArgumentNull(DispatchingRawMessageHandler sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null, new byte[0], None));
            Assert.Equal("topic", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public async Task Handle_NullMessage_ThrowsArgumentNull(DispatchingRawMessageHandler sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle("topic.name", null, None));
            Assert.Equal("message", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullMessageHandlerInvoker_ThrowsNullArgument(
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            [Frozen] ISerializer serializer)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DispatchingRawMessageHandler(messageTypeTopicMap, serializer, null));
            Assert.Equal("messageHandlerInvoker", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullMessageTypeTopicMap_ThrowsNullArgument(
            [Frozen] IMessageHandlerInvoker messageHandlerInvoker,
            [Frozen] ISerializer serializer)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DispatchingRawMessageHandler(null, serializer, messageHandlerInvoker));
            Assert.Equal("typeTopicMap", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullISerialize_ThrowsNullArgument(
            [Frozen] IMessageHandlerInvoker messageHandlerInvoker,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DispatchingRawMessageHandler(messageTypeTopicMap, null, messageHandlerInvoker));
            Assert.Equal("serializer", ex.ParamName);
        }
    }
}
