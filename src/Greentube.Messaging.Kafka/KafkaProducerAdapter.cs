using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Greentube.Messaging.Kafka
{
    internal class KafkaProducerAdapter : IKafkaProducer
    {
        public Producer<Null, byte[]> KafkaProducer { get; }

        public KafkaProducerAdapter(Producer<Null,byte[]> producer) =>
            KafkaProducer = producer ?? throw new ArgumentNullException(nameof(producer));

        public int Flush(TimeSpan timeout) => KafkaProducer.Flush(timeout);

        public Task<Message<Null, byte[]>> ProduceAsync(string topic, Null key, byte[] val) 
            => KafkaProducer.ProduceAsync(topic, key, val);

        public void Dispose() => KafkaProducer.Dispose();
    }
}
