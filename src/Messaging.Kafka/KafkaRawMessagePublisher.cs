using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Messaging.Kafka
{
    public class KafkaRawMessagePublisher : IRawMessagePublisher, IDisposable
    {
        private readonly KafkaOptions _options;
        private readonly Producer<Null, byte[]> _producer;
        private static ByteArraySerializer _byteArraySerializier = new ByteArraySerializer();

        public KafkaRawMessagePublisher(KafkaOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _producer = new Producer<Null, byte[]>(options.Properties, null, _byteArraySerializier);
            _options.Publisher.ProducerCreatedCallback?.Invoke(_producer);
        }

        public Task Publish(string topic, byte[] message, CancellationToken token)
        {
            return _producer.ProduceAsync(topic, null, message);
        }

        private class ByteArraySerializer : ISerializer<byte[]>
        {
            public byte[] Serialize(byte[] data) => data;
        }

        public void Dispose()
        {
            _producer.Flush(_options.Publisher.FlushTimeout);
            _producer.Dispose();
        }
    }
}