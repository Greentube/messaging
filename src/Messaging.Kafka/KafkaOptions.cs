using System;
using Confluent.Kafka;

namespace Messaging.Kafka
{
    public class KafkaOptions
    {
        /// <summary>
        /// Kafka Properties
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md"/>
        /// </remarks>
        public KafkaProperties Properties { get; set; } = new KafkaProperties();

        public SubscriberOptions Subscriber { get; set; } = new SubscriberOptions();
        public PublisherOptions Publisher { get; set; } = new PublisherOptions();

        public KafkaOptions()
        {

        }
    }

    public class SubscriberOptions
    {

        /// <summary>
        /// The maximum time to block.
        /// You should typically use a relatively short timout period because this operation cannot be cancelled
        /// </summary>
        public TimeSpan ConsumeTimeout { get; set; } = TimeSpan.FromSeconds(3);
        /// <summary>
        /// Invoked when a Kafka Consumer is instantiated
        /// </summary>
        public Action<Consumer<Null, byte[]>> ConsumerCreatedCallback { get; set; }
    }

    public class PublisherOptions
    {

        /// <summary>
        /// The maximum length of time to block.
        /// You should typically use a relatively short timout period because this operation cannot be cancelled.
        /// </summary>
        public TimeSpan FlushTimeout { get; set; } = TimeSpan.FromSeconds(3);
        /// <summary>
        /// Invoked when a Kafka Producer is instantiated
        /// </summary>
        public Action<Producer<Null, byte[]>> ProducerCreatedCallback { get; set; }
    }

}