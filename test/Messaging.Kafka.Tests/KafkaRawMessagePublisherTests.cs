using System;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using static System.Threading.CancellationToken;

namespace Messaging.Kafka.Tests
{
    // Since KafkaRawMessagePublisher is coupled somewhat to the concrete Producer class of Confluent.Kafka
    // the scope of these tests are reduced
    public class KafkaRawMessagePublisherTests
    {
        [Theory, AutoSubstituteData]
        public async Task Publish_CallsInnerPublisher(string topic, byte[] message)
        {
            var options = new KafkaOptions();

            var producer = Substitute.For<IKafkaProducer>();
            var sut = new KafkaRawMessagePublisher(() => producer, options);
            await sut.Publish(topic, message, None);

            await producer.Received(1).ProduceAsync(topic, null, message);
        }

        [Fact]
        public void Dispose_CallsFlushWithTimeout()
        {
            var options = new KafkaOptions
            {
                Publisher = { FlushTimeout = TimeSpan.MaxValue }
            };

            var producer = Substitute.For<IKafkaProducer>();
            var sut = new KafkaRawMessagePublisher(() => producer, options);

            sut.Dispose();

            producer.Received(1).Dispose();
            producer.Received(1).Flush(TimeSpan.MaxValue);
        }

        [Fact]
        public void Constructor_CallbackThrows_DisposesProducer()
        {
            var options = new KafkaOptions
            {
                Publisher = {ProducerCreatedCallback = _ => throw new DivideByZeroException()}
            };

            var producer = Substitute.For<IKafkaProducer>();
            Assert.Throws<DivideByZeroException>(() => new KafkaRawMessagePublisher(() => producer, options));

            producer.Received(1).Dispose();
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new KafkaRawMessagePublisher(null));
        }
    }
}
