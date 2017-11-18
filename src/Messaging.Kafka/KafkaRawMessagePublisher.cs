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
        private readonly IKafkaProducer _producer;
        private static readonly ByteArraySerializer Serializer = new ByteArraySerializer();

        /// <summary>
        /// Creates a new instance of <see cref="KafkaRawMessagePublisher"/>
        /// </summary>
        /// <param name="options"></param>
        public KafkaRawMessagePublisher(KafkaOptions options)
            : this(
                  () => new KafkaProducerAdapter(
                      new Producer<Null, byte[]>(options.Properties, null, Serializer)),
                  options) { }

        internal KafkaRawMessagePublisher(
            Func<IKafkaProducer> producerFunc,
            KafkaOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            var producer = producerFunc();
            try
            {
                _options.Publisher.ProducerCreatedCallback?.Invoke(producer.KafkaProducer);
            }
            catch
            {
                producer.Dispose();
                throw;
            }
            _producer = producer;
        }

        /// <summary>
        /// Publishes the raw message to the topic using Kafka Producer
        /// </summary>
        /// <param name="topic">The topic to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="_">The ignored cancellation token due to the implementation not supporting.</param>
        /// <inheritdoc />
        public Task Publish(string topic, byte[] message, CancellationToken _)
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