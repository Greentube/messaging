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
    public class RedisRawMessageHandlerSubscriber : IRawMessageHandlerSubscriber
    {
        private readonly IConnectionMultiplexer _mux;

        public RedisRawMessageHandlerSubscriber(IConnectionMultiplexer mux) =>
            _mux = mux ?? throw new ArgumentNullException(nameof(mux));

        /// <summary>
        /// Subscribes to the specified topic with Redis Pub/Sub
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="rawHandler"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        public Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken _)
        {
            void HandleRedisMessage(RedisChannel channel, RedisValue value) =>
                rawHandler.Handle(channel, value, None)
                    .GetAwaiter()
                    .GetResult();

            var subscriber = _mux.GetSubscriber();
            return subscriber.SubscribeAsync(topic, HandleRedisMessage);
        }
    }
}