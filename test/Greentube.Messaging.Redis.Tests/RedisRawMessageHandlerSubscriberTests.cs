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
    public class RedisRawMessageHandlerSubscriberTests
    {
        [Theory, AutoSubstituteData]
        public async Task Subscribe_SubscribersRawMessageHandler(
            [Frozen] ISubscriber subscriber,
            string topic,
            byte[] message,
            IRawMessageHandler rawMessageHandler,
            RedisRawMessageHandlerSubscriber sut)
        {
            Action<RedisChannel, RedisValue> redisCallback = null;
            await subscriber.SubscribeAsync(topic, Arg.Do<Action<RedisChannel, RedisValue>>(action => redisCallback = action));

            await sut.Subscribe(topic, rawMessageHandler, None);

            Assert.NotNull(redisCallback);
            redisCallback(topic, message);
            await rawMessageHandler.Received(1).Handle(topic, message, None);
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_MultiplexerReturnsNoSubscription_ThrowsInvalidOperation(
            [Frozen] IConnectionMultiplexer connectionMultiplexer,
            string topic,
            IRawMessageHandler rawMessageHandler,
            RedisRawMessageHandlerSubscriber sut)
        {
            connectionMultiplexer.GetSubscriber().ReturnsNull();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Subscribe(topic, rawMessageHandler, None));
            Assert.Equal("Redis Multiplexer returned no subscription.", ex.Message);
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_NullTopic_ThrowsArgumentNull(
            IRawMessageHandler rawMessageHandler,
            RedisRawMessageHandlerSubscriber sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Subscribe(null, rawMessageHandler, None));
            Assert.Equal("topic", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_NullRawMessageHandler_ThrowsArgumentNull(
            string topic,
            RedisRawMessageHandlerSubscriber sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Subscribe(topic, null, None));
            Assert.Equal("rawHandler", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullMultiplexer_ThrowsArgumentNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RedisRawMessageHandlerSubscriber(null));
            Assert.Equal("connectionMultiplexer", ex.ParamName);
        }
    }
}
