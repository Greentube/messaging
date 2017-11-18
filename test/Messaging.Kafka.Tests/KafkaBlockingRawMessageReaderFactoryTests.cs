using System;
using Confluent.Kafka;
using NSubstitute;
using Xunit;

namespace Messaging.Kafka.Tests
{
    public class KafkaBlockingRawMessageReaderFactoryTests
    {
        private readonly KafkaBlockingRawMessageReaderFactory _sut = new KafkaBlockingRawMessageReaderFactory();

        [Fact]
        public void Create_DisposesOnCallbackError()
        {
            var consumer = Substitute.For<IKafkaConsumer>();
            Assert.Throws<DivideByZeroException>(() => _sut.Create(() => consumer, "topic", new KafkaOptions
            {
                Properties = { GroupId = "groupId" },
                Subscriber =
                {
                    ConsumerCreatedCallback = _ =>
                        throw new DivideByZeroException()
                }
            }));

            consumer.Received(1).Dispose();
        }

        [Theory, AutoSubstituteData]
        public void Create_NullCallback_Subscribes(string topic)
        {
            var consumer = Substitute.For<IKafkaConsumer>();
            _sut.Create(() => consumer, topic, new KafkaOptions
            {
                Properties = { GroupId = "groupId" }
            });

            consumer.Received(1).Subscribe(topic);
        }

        [Fact]
        public void Create_ReturnsKafkaBlockingRawMessageReader()
        {
            var reader = _sut.Create("topic", new KafkaOptions
            {
                Properties = { GroupId = "groupId" },
                Subscriber = { ConsumerCreatedCallback = consumer =>
                Assert.Equal(typeof(Consumer<Null, byte[]>), consumer.GetType()) }
            });

            try
            {
                Assert.Equal(typeof(KafkaBlockingRawMessageReader), reader.GetType());
            }
            finally
            {
                (reader as IDisposable)?.Dispose();
            }
        }

        [Fact]
        public void Create_NullTopic_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create(null, new KafkaOptions()));
        }

        [Fact]
        public void Create_NullTopic_NoReaderCreated()
        {
            bool called = false;

            Assert.Throws<ArgumentNullException>(() =>
                _sut.Create(() =>
                {
                    called = false;
                    return null;
                }, null, new KafkaOptions()));

            Assert.False(called);
        }

        [Fact]
        public void Create_NullOptions_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create("topic", null));
        }
    }
}
