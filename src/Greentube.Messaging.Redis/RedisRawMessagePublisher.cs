using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Greentube.Messaging.Redis
{
    /// <summary>
    /// A Redis Raw Message Publisher
    /// </summary>
    /// <inheritdoc />
    public class RedisRawMessagePublisher : IRawMessagePublisher
    {
        private readonly ISubscriber _subscriber;

        /// <summary>
        /// Creates an instance of <see cref="RedisRawMessagePublisher"/>
        /// </summary>
        /// <param name="connectionMultiplexer">Redis ConnectionMultiplexer</param>
        public RedisRawMessagePublisher(IConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer == null) throw new ArgumentNullException(nameof(connectionMultiplexer));
            _subscriber = connectionMultiplexer.GetSubscriber()
                ?? throw new ArgumentException("Redis Multiplexer returned no subscription.", nameof(connectionMultiplexer));
        }

        /// <summary>
        /// Publishes the raw message to the topic using Redis Pub/Sub
        /// </summary>
        /// <param name="topic">The topic to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="_">Ignored token as SE.Redis doesn't support it.</param>
        /// <inheritdoc />
        public Task Publish(string topic, byte[] message, CancellationToken _)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (message == null) throw new ArgumentNullException(nameof(message));

            return _subscriber.PublishAsync(topic, message);
        }
    }
}