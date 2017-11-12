using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using static System.Threading.CancellationToken;

namespace Messaging.Redis
{
    /// <summary>
    /// A Redis Raw Message Handler Subscriber
    /// </summary>
    /// <inheritdoc />
    public class RedisRawMessageHandlerSubscriber : IRawMessageHandlerSubscriber
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisRawMessageHandlerSubscriber(IConnectionMultiplexer connectionMultiplexer) =>
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));

        /// <summary>
        /// Subscribes to the specified topic with Redis Pub/Sub
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="rawHandler"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken _)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (rawHandler == null) throw new ArgumentNullException(nameof(rawHandler));

            void HandleRedisMessage(RedisChannel channel, RedisValue value) =>
                rawHandler.Handle(channel, value, None)
                    .GetAwaiter()
                    .GetResult();

            var subscriber = _connectionMultiplexer.GetSubscriber()
                ?? throw new InvalidOperationException("Redis Multiplexer returned no subscription.");

            return subscriber.SubscribeAsync(topic, HandleRedisMessage);
        }
    }
}