using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Messaging.Redis
{
    /// <summary>
    /// A Redis Raw Message Publisher
    /// </summary>
    /// <inheritdoc />
    public class RedisRawMessagePublisher : IRawMessagePublisher
    {
        private readonly ISubscriber _subscriber;

        public RedisRawMessagePublisher(IConnectionMultiplexer mux) =>
            _subscriber = mux?.GetSubscriber() ?? throw new ArgumentException("No subscription available.", nameof(mux));

        /// <summary>
        /// Publishes the raw message to the topic using Redis Pub/Sub
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public Task Publish(string topic, byte[] message, CancellationToken _)
        {
            return _subscriber.PublishAsync(topic, message);
        }
    }
}