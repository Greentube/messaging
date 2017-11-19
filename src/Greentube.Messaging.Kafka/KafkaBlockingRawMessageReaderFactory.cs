using System;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Greentube.Messaging.Kafka
{
    /// <summary>
    /// Creates instances of <see cref="T:Messaging.Kafka.KafkaBlockingRawMessageReader" /> which are already subscribed to the specified topic
    /// </summary>
    /// <inheritdoc />
    public class KafkaBlockingRawMessageReaderFactory : IBlockingRawMessageReaderFactory<KafkaOptions>
    {
        private static readonly ByteArrayDeserializer Deserializer = new ByteArrayDeserializer();

        /// <summary>
        /// Creates a new <see cref="T:Messaging.Kafka.KafkaBlockingRawMessageReader" /> subscribed already to the specified <param name="topic" />
        /// </summary>
        /// <param name="topic">The topic to subscribe the reader</param>
        /// <param name="options">Kafka Options</param>
        /// <returns><see cref="T:Messaging.Kafka.KafkaBlockingRawMessageReader" /> subscribed to <param name="topic" /></returns>
        /// <inheritdoc />
        public IBlockingRawMessageReader<KafkaOptions> Create(string topic, KafkaOptions options)
        {
            IKafkaConsumer ConsumerFunc() => 
                new KafkaConsumerAdapter(new Consumer<Null, byte[]>(options.Properties, null, Deserializer));

            return Create(ConsumerFunc, topic, options);
        }

        internal IBlockingRawMessageReader<KafkaOptions> Create(
            Func<IKafkaConsumer> consumerFunc,
            string topic, 
            KafkaOptions options)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var consumer = consumerFunc();
            try
            {
                options.Subscriber.ConsumerCreatedCallback?.Invoke(consumer.KafkaConsumer);
                consumer.Subscribe(topic);
            }
            catch
            {
                consumer.Dispose();
                throw;
            }

            return new KafkaBlockingRawMessageReader(consumer);
        }

        private class ByteArrayDeserializer : IDeserializer<byte[]>
        {
            public byte[] Deserialize(byte[] data) => data;
        }
    }
}
