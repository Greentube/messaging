using System;
using Confluent.Kafka;
using NSubstitute;
using Xunit;

namespace Messaging.Kafka.Tests
{
    public class KafkaBlockingRawMessageReaderTests
    {
        [Fact]
        public void TryGetMessage_ConsumerReturnsFalse_ReaderReturnsFalse()
        {
            // Arrange
            var options = new KafkaOptions();

            var consumer = Substitute.For<IKafkaConsumer>();
            consumer.Consume(out _, Arg.Any<TimeSpan>())
                .Returns(r => {
                    r[0] = null;
                    return false;
                });

            // Act
            var sut = new KafkaBlockingRawMessageReader(consumer);
            var read = sut.TryGetMessage(out var readMessage, options);

            // Assert
            Assert.False(read);
            Assert.Null(readMessage);
        }

        [Theory, AutoSubstituteData]
        public void TryGetMessage_ReturnsConsumerMessage(byte[] expectedMessage, string topic)
        {
            // Arrange
            var options = new KafkaOptions();

            var consumer = Substitute.For<IKafkaConsumer>();
            consumer.Consume(out _, Arg.Any<TimeSpan>())
                .Returns(r => {
                    r[0] = new Message<Null, byte[]>(topic, 0, 0, null, expectedMessage, new Timestamp(), null);
                    return true;
                });

            // Act
            var sut = new KafkaBlockingRawMessageReader(consumer);
            var read = sut.TryGetMessage(out var actualMessage, options);

            // Assert
            Assert.True(read);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TryGetMessage_NullOptions_ThrowsArgumentNull()
        {
            var consumer = Substitute.For<IKafkaConsumer>();

            var sut = new KafkaBlockingRawMessageReader(consumer);

            Assert.Throws<ArgumentNullException>(() => sut.TryGetMessage(out var _, null));
        }

        [Fact]
        public void Dispose_DisposesKafkaConsumer()
        {
            var consumer = Substitute.For<IKafkaConsumer>();
            var sut = new KafkaBlockingRawMessageReader(consumer);

            sut.Dispose();

            consumer.Received(1).Dispose();
        }

        [Fact]
        public void Constructor_NullConsumer_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new KafkaBlockingRawMessageReader(null));
        }
    }
}