using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka
{
    public class KafkaRawMessagePublisher : IRawMessagePublisher, IDisposable
    {
        private readonly Producer<Null, byte[]> _producer;

        public KafkaRawMessagePublisher(IOptions<KafkaOptions> options)
        {
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));
            _producer = new Producer<Null, byte[]>(options.Value, null, new ByteArraySerializer());
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
            _producer.Dispose();
        }
    }
}