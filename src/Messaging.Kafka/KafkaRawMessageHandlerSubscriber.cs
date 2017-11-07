using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Messaging.Kafka
{
    public class KafkaRawMessageHandlerSubscriber : IRawMessageHandlerSubscriber
    {
        private readonly ConcurrentDictionary<string,
            (Task task, CancellationTokenSource tokenSource, Consumer<Null, byte[]> consumer)> _consumers
            = new ConcurrentDictionary<string,
                (Task task, CancellationTokenSource tokenSource, Consumer<Null, byte[]> consumer)>();

        private readonly KafkaOptions _options;

        public KafkaRawMessageHandlerSubscriber(KafkaOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken _)
        {
            var subscriptionTask = new TaskCompletionSource<bool>();
            _consumers.AddOrUpdate(topic, t =>
            {
                var consumerCancellation = new CancellationTokenSource();

                var consumer = new Consumer<Null, byte[]>(_options, null, new ByteArrayDeserializer());

                // A task per topic for now
                var consumerTask = Task.Run(() =>
                {
                    try
                    {
                        consumer.Subscribe(topic);
                        subscriptionTask.SetResult(true); // Complete the subscription task

                        while (!consumerCancellation.IsCancellationRequested)
                        {
                            if (consumer.Consume(out var msg, TimeSpan.FromSeconds(1)))
                            {
                                rawHandler.Handle(msg.Topic, msg.Value, consumerCancellation.Token)
                                    .GetAwaiter()
                                    .GetResult();
                            }
                        }
                    }
                    finally
                    {
                        consumer.Dispose();
                    }
                }, consumerCancellation.Token);
                return (consumerTask, consumerCancellation, consumer);
            },
            (s, tuple) => tuple);

            return subscriptionTask.Task;
        }

        // TODO: Unsubscribe? Cancel token and remove consumer from dictionary

        private class ByteArrayDeserializer : IDeserializer<byte[]>
        {
            public byte[] Deserialize(byte[] data) => data;
        }
    }
}