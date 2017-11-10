using Messaging.DependencyInjection;
using Messaging.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisMessagingBuilderExtensions
    {
        /// <summary>
        /// Adds both Redis publisher and subscriber
        /// </summary>
        /// <remarks>
        /// These services expect <see cref="IConnectionMultiplexer"/> to be registered
        /// </remarks>
        /// <param name="builder">Messaging builder</param>
        /// <returns></returns>
        public static MessagingBuilder AddRedis(this MessagingBuilder builder)
        {
            builder.AddRedisPublisher();
            builder.AddRedisSubscriber();
            return builder;
        }

        /// <summary>
        /// Adds both Redis publisher and subscriber with <see cref="IConnectionMultiplexer"/>
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <param name="multiplexer">StackExchange.Redis ConnectionMultiplexer</param>
        /// <returns></returns>
        public static MessagingBuilder AddRedis(this MessagingBuilder builder, IConnectionMultiplexer multiplexer)
        {
            builder.Services.TryAddSingleton(multiplexer);
            return builder.AddRedis();
        }

        /// <summary>
        /// Register Redis raw publisher service
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <returns></returns>
        public static MessagingBuilder AddRedisPublisher(this MessagingBuilder builder)
        {
            builder.AddRawMessagePublisher<RedisRawMessagePublisher>();
            return builder;
        }

        /// <summary>
        /// Register Redis raw subscriber service
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <returns></returns>
        public static MessagingBuilder AddRedisSubscriber(this MessagingBuilder builder)
        {
            builder.AddRawMessageHandlerSubscriber<RedisRawMessageHandlerSubscriber>();
            return builder;
        }
    }
}
