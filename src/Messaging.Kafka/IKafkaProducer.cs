using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Messaging.Kafka
{
    internal interface IKafkaProducer: IDisposable
    {
        Task<Message<Null, byte[]>> ProduceAsync(string topic, Null key, byte[] val);
        int Flush(TimeSpan timeout);
        Producer<Null, byte[]> KafkaProducer { get; }
    }
}