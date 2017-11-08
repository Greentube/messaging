using System;
using Confluent.Kafka;

namespace Messaging.Kafka
{
    /// <summary>
    /// Publisher options
    /// </summary>
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