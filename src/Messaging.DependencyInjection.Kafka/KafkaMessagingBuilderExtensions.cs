using System;
using Messaging.DependencyInjection;
using Messaging.Kafka;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaMessagingBuilderExtensions
    {
        public static MessagingBuilder AddKafka(this MessagingBuilder builder)
        {
            builder.AddRawMessagePublisher<KafkaRawMessagePublisher>();
            builder.AddRawMessageHandlerSubscriber<KafkaRawMessageHandlerSubscriber>();
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
