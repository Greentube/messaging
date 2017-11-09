using Messaging.DependencyInjection;
using Messaging.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisMessagingBuilderExtensions
    {
        // Adds redis assuming a multiplexer has been previously registered
        public static MessagingBuilder AddRedis(this MessagingBuilder builder)
        {
            builder.AddRawMessagePublisher<RedisRawMessagePublisher>();
            builder.AddRawMessageHandlerSubscriber<RedisRawMessageHandlerSubscriber>();
            return builder;
        }

        public static MessagingBuilder AddRedis(this MessagingBuilder builder, IConnectionMultiplexer multiplexer)
        {
            builder.Services.TryAddSingleton(multiplexer);
            return builder.AddRedis();
        }
    }
}
