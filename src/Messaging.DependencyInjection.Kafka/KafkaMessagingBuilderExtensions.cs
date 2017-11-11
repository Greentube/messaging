using System;
using Messaging.DependencyInjection;
using Messaging.Kafka;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace - Discoverability
namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaMessagingBuilderExtensions
    {
        /// <summary>
        /// Adds both Kafka publisher and subscriber
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <param name="actionSetup">Setup Kafka options</param>
        /// <returns></returns>
        public static MessagingBuilder AddKafka(this MessagingBuilder builder, Action<KafkaOptions> actionSetup)
        {
            builder.Services.Configure(actionSetup);
            return builder.AddKafka();
        }

        /// <summary>
        /// Adds both Kafka publisher and subscriber
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <returns></returns>
        public static MessagingBuilder AddKafka(this MessagingBuilder builder)
        {
            builder.AddKafkaPublisher();
            builder.AddKafkaSubscriber();
            return builder;
        }

        /// <summary>
        /// Register Kafka raw publisher service
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <param name="actionSetup">Setup Kafka options</param>
        /// <returns></returns>
        public static MessagingBuilder AddKafkaPublisher(this MessagingBuilder builder, Action<KafkaOptions> actionSetup)
        {
            builder.Services.Configure(actionSetup);
            builder.AddKafkaPublisher();
            return builder;
        }

        /// <summary>
        /// Register Kafka raw subscriber service
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <param name="actionSetup">Setup Kafka options</param>
        /// <returns></returns>
        public static MessagingBuilder AddKafkaSubscriber(this MessagingBuilder builder, Action<KafkaOptions> actionSetup)
        {
            builder.Services.Configure(actionSetup);
            builder.AddKafkaSubscriber();
            return builder;
        }

        /// <summary>
        /// Register Kafka raw publisher service
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <returns></returns>
        public static MessagingBuilder AddKafkaPublisher(this MessagingBuilder builder)
        {
            builder.Services.TryAddSingleton(c => c.GetRequiredService<IOptions<KafkaOptions>>().Value);
            builder.AddRawMessagePublisher<KafkaRawMessagePublisher>();
            return builder;
        }

        /// <summary>
        /// Register Kafka raw subscriber service
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <returns></returns>
        public static MessagingBuilder AddKafkaSubscriber(this MessagingBuilder builder)
        {
            builder.Services.TryAddSingleton(c => c.GetRequiredService<IOptions<KafkaOptions>>().Value);
            builder.AddRawMessageHandlerSubscriber<KafkaRawMessageHandlerSubscriber>();
            return builder;
        }
    }
}
