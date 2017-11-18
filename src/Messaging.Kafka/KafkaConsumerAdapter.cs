using System;
using Confluent.Kafka;

namespace Messaging.Kafka
{
    internal class KafkaConsumerAdapter : IKafkaConsumer
    {
        public Consumer<Null, byte[]> KafkaConsumer { get; }

        public KafkaConsumerAdapter(Consumer<Null, byte[]> consumer) =>
            KafkaConsumer = consumer ?? throw new ArgumentNullException(nameof(consumer));

        public bool Consume(out Message<Null, byte[]> message, TimeSpan timeout)
            => KafkaConsumer.Consume(out message, timeout);

        public void Subscribe(string topic) => KafkaConsumer.Subscribe(topic);

        public void Dispose() => KafkaConsumer.Dispose();
    }
}