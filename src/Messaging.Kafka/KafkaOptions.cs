using System;

namespace Messaging.Kafka
{
    /// <summary>
    /// Kafka Options
    /// </summary>
    /// <inheritdoc />
    public class KafkaOptions : PollingReaderOptions
    {
        /// <summary>
        /// Kafka Properties
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md"/>
        /// </remarks>
        public KafkaProperties Properties { get; set; } = new KafkaProperties();
        /// <summary>
        /// Subscriber options
        /// </summary>
        public SubscriberOptions Subscriber { get; set; } = new SubscriberOptions();
        /// <summary>
        /// Publisher options
        /// </summary>
        public PublisherOptions Publisher { get; set; } = new PublisherOptions();
        /// <summary>
        /// There's no need to block between reads as it's done by the implementation of Consumer.Consume
        /// </summary>
        /// <inheritdoc />
        public override TimeSpan SleepBetweenPolling => default(TimeSpan);
    }
}