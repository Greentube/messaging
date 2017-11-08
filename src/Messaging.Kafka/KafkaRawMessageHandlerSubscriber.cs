using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Messaging.Kafka
{
    public class KafkaRawMessageHandlerSubscriber : IRawMessageHandlerSubscriber, IDisposable
    {
        private readonly ConcurrentDictionary<string,
            (Task task, CancellationTokenSource tokenSource)> _consumers
            = new ConcurrentDictionary<string,
                (Task task, CancellationTokenSource tokenSource)>();

        private readonly KafkaOptions _options;

        public KafkaRawMessageHandlerSubscriber(KafkaOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken subscriptionCancellation)
        {
            var subscriptionTask = new TaskCompletionSource<bool>();

            _consumers.AddOrUpdate(
                topic,
                CreateConsumer(topic, rawHandler, subscriptionTask, subscriptionCancellation),
                (_, tuple) =>
                {
                    if (tuple.task.IsFaulted || tuple.task.IsCompleted)
                    {
                        return CreateConsumer(topic, rawHandler, subscriptionTask, subscriptionCancellation);
                    }

                    subscriptionTask.SetResult(true);
                    return tuple;
                });

            return subscriptionTask.Task;
        }

        private (Task task, CancellationTokenSource token) CreateConsumer(
            string topic,
            IRawMessageHandler rawHandler,
            TaskCompletionSource<bool> subscriptionTask,
            CancellationToken subscriptionCancellation)
        {
            // Consumer cancellation will let us stop this task on Dispose/Unsubscribe
            var consumerCancellation = new CancellationTokenSource();

            // A task per subscription for now - This should vary. Could be single worker thread or .. N
            var consumerTask = Task.Run((Action) ConsumerTaskCreation,
                // The Consumer will handle the cancellation by gracefully shutting down.
                CancellationToken.None);

            return (consumerTask, consumerCancellation);

            void ConsumerTaskCreation()
            {
                if (subscriptionCancellation.IsCancellationRequested)
                {
                    subscriptionTask.SetCanceled();
                    return;
                }

                Consumer<Null, byte[]> consumer = null;
                try
                {
                    consumer = new Consumer<Null, byte[]>(_options.Properties, null, new ByteArrayDeserializer());
                    _options.Subscriber.ConsumerCreatedCallback?.Invoke(consumer);
                    consumer.Subscribe(topic);
                }
                catch (Exception e)
                {
                    subscriptionTask.SetException(e);
                    consumer?.Dispose();
                    return;
                }

                subscriptionTask.SetResult(true);

                Consume(rawHandler, consumer, _options, consumerCancellation.Token);
            }
        }

        private static void Consume(
            IRawMessageHandler rawHandler,
            Consumer<Null, byte[]> consumer,
            KafkaOptions kafkaOptions,
            CancellationToken consumerCancellation)
        {
            try
            {
                while (!consumerCancellation.IsCancellationRequested)
                {
                    if (consumer.Consume(out var msg, kafkaOptions.Subscriber.ConsumeTimeout))
                    {
                        rawHandler.Handle(msg.Topic, msg.Value, consumerCancellation)
                            .GetAwaiter()
                            .GetResult();
                    }
                }
            }
            finally
            {
                consumer.Dispose();
            }
        }

        public Task Unsubscribe(string topic, IRawMessageHandler _, CancellationToken __)
        {
            if (_consumers.TryRemove(topic, out var tuple))
            {
                tuple.tokenSource.Cancel();
                tuple.tokenSource.Dispose();
            }

            return tuple.task;
        }

        private class ByteArrayDeserializer : IDeserializer<byte[]>
        {
            public byte[] Deserialize(byte[] data) => data;
        }

        public void Dispose()
        {
            foreach (var topic in _consumers.Keys)
            {
                try
                {
                    Unsubscribe(topic, null, CancellationToken.None)
                        .GetAwaiter()
                        .GetResult();
                }
                catch // CA1065
                {
                }
            }
        }
    }
}