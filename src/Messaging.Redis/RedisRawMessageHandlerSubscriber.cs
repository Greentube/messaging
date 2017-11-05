using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using static System.Threading.CancellationToken;

namespace Messaging.Redis
{
    public class RedisRawMessageHandlerSubscriber : IRawMessageHandlerSubscriber
    {
        private readonly IConnectionMultiplexer _mux;

        public RedisRawMessageHandlerSubscriber(IConnectionMultiplexer mux)
        {
            _mux = mux ?? throw new ArgumentNullException(nameof(mux));
        }

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