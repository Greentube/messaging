using System;
using Confluent.Kafka;

namespace Greentube.Messaging.Kafka
{
    internal interface IKafkaConsumer : IDisposable
    {
        Consumer<Null, byte[]> KafkaConsumer { get; }
        bool Consume(out Message<Null, byte[]> message, TimeSpan timeout);
        void Subscribe(string topic);
    }
}