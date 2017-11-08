using System;
using Messaging;
using Messaging.DependencyInjection;
using Messaging.Kafka;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaMessagingBuilderExtensions
    {
        public static MessagingBuilder AddKafka(this MessagingBuilder builder)
        {
            var services = builder.Services;
            services.TryAddSingleton<IRawMessagePublisher, KafkaRawMessagePublisher>();
            services.TryAddSingleton<IRawMessageHandlerSubscriber, KafkaRawMessageHandlerSubscriber>();
            builder.Services.AddSingleton(c => c.GetRequiredService<IOptions<KafkaOptions>>().Value);

            return builder;
        }

        public static MessagingBuilder AddKafka(this MessagingBuilder builder, Action<KafkaOptions> actionSetup)
        {
            builder.Services.Configure(actionSetup);
            return builder.AddKafka();
        }
    }
}
