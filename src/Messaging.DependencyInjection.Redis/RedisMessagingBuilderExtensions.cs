using Messaging;
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
            var services = builder.Services;
            services.TryAddSingleton<IRawMessagePublisher, RedisRawMessagePublisher>();
            services.TryAddSingleton<IRawMessageHandlerSubscriber, RedisRawMessageHandlerSubscriber>();
            services.TryAddSingleton<IRawMessageHandler, DispatchingRawMessageHandler>();
            services.TryAddSingleton<IMessageHandlerInfoProvider, MessageHandlerInfoProvider>();
            return builder;
        }

        public static MessagingBuilder AddRedis(this MessagingBuilder builder, IConnectionMultiplexer multiplexer)
        {
            builder.Services.TryAddSingleton(multiplexer);
            return builder.AddRedis();
        }
    }
}
