using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Greentube.Serialization;
using Xunit;
using static System.Threading.CancellationToken;

namespace Greentube.Messaging.Tests
{
    public class SerializedMessagePublisherTests
    {
        [Theory, AutoSubstituteData]
        public async Task Publish_SerializedMessage_CallsRawPublisher(
            string topic,
            bool message,
            byte[] serializedMessage,
            [Frozen] IRawMessagePublisher rawMessagePublisher,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            [Frozen] ISerializer serializer,
            SerializedMessagePublisher sut)
        {
            // Arrange
            var messageType = message.GetType();
            messageTypeTopicMap.Get(messageType).Returns(topic);
            serializer.Serialize(message).Returns(serializedMessage);

            // Act
            await sut.Publish(message, None);

            // Assert
            await rawMessagePublisher.Received().Publish(topic, serializedMessage, None);
        }

        [Theory, AutoSubstituteData]
        public async Task Publish_SerializerReturnsNull_ThrowsInvalidOperation(
            string topic,
            string message,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            [Frozen] ISerializer serializer,
            SerializedMessagePublisher sut)
        {
            var messageType = GetType();
            messageTypeTopicMap.Get(topic).Returns(messageType);
            serializer.Serialize( message).ReturnsNull();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Publish(message, None));
            Assert.Equal(
                $"Serializer {serializer.GetType()} returned null for message of type {message.GetType()}.",
                ex.Message);
        }

        [Theory, AutoSubstituteData]
        public async Task Publish_TopicMapNotDefined_ThrowsInvalidOperation(
            string message,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            SerializedMessagePublisher sut)
        {
            messageTypeTopicMap.Get(message.GetType()).ReturnsNull();
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Publish(message, None));
            Assert.Equal($"Message type {message.GetType()} is not registered with: {messageTypeTopicMap.GetType()}.",
                ex.Message);
        }

        [Theory, AutoSubstituteData]
        public async Task Publish_NullMessage_ThrowsArgumentNull(SerializedMessagePublisher sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Publish(null as object, None));
            Assert.Equal("message", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullRawMessagePublisher_ThrowsNullArgument(
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            [Frozen] ISerializer serializer)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SerializedMessagePublisher(messageTypeTopicMap, serializer, null));
            Assert.Equal("rawMessagePublisher", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullMessageTypeTopicMap_ThrowsNullArgument(
            [Frozen] IRawMessagePublisher rawMessagePublisher,
            [Frozen] ISerializer serializer)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SerializedMessagePublisher(null, serializer, rawMessagePublisher));
            Assert.Equal("typeTopicMap", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullISerialize_ThrowsNullArgument(
            [Frozen] IRawMessagePublisher rawMessagePublisher,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SerializedMessagePublisher(messageTypeTopicMap, null, rawMessagePublisher));
            Assert.Equal("serializer", ex.ParamName);
        }
    }
}