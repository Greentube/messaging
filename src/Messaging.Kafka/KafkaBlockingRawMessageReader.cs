using System;

namespace Messaging.Kafka
{
    /// <summary>
    /// An Apache Kafka implementation of <see cref="T:Messaging.IBlockingRawMessageReader`1" />
    /// </summary>
    /// <inheritdoc cref="IBlockingRawMessageReader{KafkaOptions}" />
    /// <inheritdoc cref="IDisposable" />
    public class KafkaBlockingRawMessageReader : IBlockingRawMessageReader<KafkaOptions>, IDisposable
    {
        private readonly IKafkaConsumer _consumer;

        /// <summary>
        /// Creates an new instance of <see cref="KafkaBlockingRawMessageReader"/>
        /// </summary>
        /// <param name="consumer"></param>
        internal KafkaBlockingRawMessageReader(IKafkaConsumer consumer) => 
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));

        /// <summary>
        /// Tries to read a message from the inner Confluent.Consumer implementation
        /// </summary>
        /// <param name="message">The message read if true was returned</param>
        /// <param name="options">Kafka options</param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool TryGetMessage(out byte[] message, KafkaOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var read = _consumer.Consume(out var kafkaMessage, options.Subscriber.ConsumeTimeout);
            message = read ? kafkaMessage.Value : null;
            return read;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}