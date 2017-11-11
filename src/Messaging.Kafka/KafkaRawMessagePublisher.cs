using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Messaging.Kafka
{
    /// <summary>
    /// A Kafka Raw Message Publisher
    /// </summary>
    /// <inheritdoc cref="IRawMessagePublisher" />
    /// <inheritdoc cref="IDisposable" />
    public class KafkaRawMessagePublisher : IRawMessagePublisher, IDisposable
    {
        private readonly KafkaOptions _options;
        private readonly Producer<Null, byte[]> _producer;
        private static readonly ByteArraySerializer Serializer = new ByteArraySerializer();

        public KafkaRawMessagePublisher(KafkaOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _producer = new Producer<Null, byte[]>(options.Properties, null, Serializer);
            _options.Publisher.ProducerCreatedCallback?.Invoke(_producer);
        }

        /// <summary>
        /// Publishes the raw message to the topic using Kafka Producer
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public Task Publish(string topic, byte[] message, CancellationToken token)
        {
            return _producer.ProduceAsync(topic, null, message);
        }

        private class ByteArraySerializer : ISerializer<byte[]>
        {
            public byte[] Serialize(byte[] data) => data;
        }

        /// <summary>
        /// Flushes the Producer data with the <see cref="KafkaOptions"/> timeout and disposes it.
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            _producer.Flush(_options.Publisher.FlushTimeout);
            _producer.Dispose();
        }
    }
}