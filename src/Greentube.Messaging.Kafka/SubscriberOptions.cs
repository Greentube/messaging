using System;
using Confluent.Kafka;

namespace Greentube.Messaging.Kafka
{
    /// <summary>
    /// Subscriber options
    /// </summary>
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
}