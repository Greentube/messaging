using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using StackExchange.Redis;
using Xunit;
using static System.Threading.CancellationToken;

namespace Greentube.Messaging.Redis.Tests
{
    public class RedisRawMessagePublisherTests
    {
        [Theory, AutoSubstituteData]
        public async Task Publish_DataPassedToRedisPublisher(
            [Frozen] ISubscriber subscriber,
            string topic,
            byte[] message,
            RedisRawMessagePublisher sut)
        {
            await sut.Publish(topic, message, None);

            await subscriber.Received(1).PublishAsync(topic, message);
        }

        [Theory, AutoSubstituteData]
        public async Task Publish_NullMessage_ThrowsArgumentNull(RedisRawMessagePublisher sut, string topic)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Publish(topic, null, None));
            Assert.Equal("message", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public async Task Publish_NullTopic_ThrowsArgumentNull(RedisRawMessagePublisher sut, byte[] message)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Publish(null, message, None));
            Assert.Equal("topic", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_MultiplexerReturnsNoSubscription_ThrowsArgumentException(
            IConnectionMultiplexer connectionMultiplexer)
        {
            connectionMultiplexer.GetSubscriber().ReturnsNull();

            var ex = Assert.Throws<ArgumentException>(() => new RedisRawMessagePublisher(connectionMultiplexer));

            Assert.Equal("connectionMultiplexer", ex.ParamName);
            Assert.Equal($@"Redis Multiplexer returned no subscription.
Parameter name: {ex.ParamName}", ex.Message);
        }

        [Fact]
        public void Constructor_NullMultiplexer_ThrowsArgumentNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RedisRawMessagePublisher(null));
            Assert.Equal("connectionMultiplexer", ex.ParamName);
        }
    }
}
