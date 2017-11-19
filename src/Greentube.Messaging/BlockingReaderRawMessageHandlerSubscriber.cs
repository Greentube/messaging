using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// An adapter to a blocking/polling Raw Message Handler Subscriber
    /// </summary>
    /// <remarks>
    /// Creates a task per subscription to call a blocking <see cref="IBlockingRawMessageReader{TOptions}"/> for messages
    /// A call to subscribe will return a task which completes when the subcription is done.
    /// A subscription is considered to be done when a <see cref="IBlockingRawMessageReader{TOptions}"/> is created
    /// using the provided <see cref="IBlockingRawMessageReaderFactory{TOptions}"/> factory
    /// </remarks>
    /// <inheritdoc cref="IRawMessageHandlerSubscriber" />
    /// <inheritdoc cref="IDisposable" />
    public class BlockingReaderRawMessageHandlerSubscriber<TOptions> : IRawMessageHandlerSubscriber, IDisposable
        where TOptions : IPollingOptions
    {
        private readonly TOptions _options;
        private readonly IBlockingRawMessageReaderFactory<TOptions> _factory;

        private readonly ConcurrentDictionary<(string topic, IRawMessageHandler rawHandler),
            (Task task, CancellationTokenSource tokenSource)> _readers
            = new ConcurrentDictionary<(string topic, IRawMessageHandler rawHandler),
                (Task task, CancellationTokenSource tokenSource)>();

        private readonly ConcurrentDictionary<string, object> _readerCreationLocks
            = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Create a new instance of <see cref="BlockingReaderRawMessageHandlerSubscriber{TOptions}"/>
        /// </summary>
        /// <param name="factory">The factory to be called on each new subscription</param>
        /// <param name="options">The polling options</param>
        public BlockingReaderRawMessageHandlerSubscriber(
            IBlockingRawMessageReaderFactory<TOptions> factory,
            TOptions options)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _options = options != null ? options : throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Subscribes to the specified topic with the provided blocking/polling-based raw message handler
        /// </summary>
        /// <remarks>
        /// A Task is created to call the blocking <see cref="IBlockingRawMessageReader{TOptions}"/>"/>.
        /// </remarks>
        /// <param name="topic">The topic to subscribe to</param>
        /// <param name="rawHandler">The raw handler to invoke with the bytes received</param>
        /// <param name="subscriptionCancellation">A token to cancel the topic subscription</param>
        /// <returns>Task that completes when the subscription process is finished</returns>
        /// <inheritdoc />
        public Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken subscriptionCancellation)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (rawHandler == null) throw new ArgumentNullException(nameof(rawHandler));

            var subscriptionTask = new TaskCompletionSource<bool>();

            _readers.AddOrUpdate(
                (topic, rawHandler),
                CreateReader(topic, rawHandler, subscriptionTask, subscriptionCancellation),
                (_, tuple) =>
                {
                    if (tuple.task.IsFaulted || tuple.task.IsCompleted)
                    {
                        return CreateReader(topic, rawHandler, subscriptionTask, subscriptionCancellation);
                    }

                    subscriptionTask.SetResult(true);
                    return tuple;
                });

            return subscriptionTask.Task;
        }

        private (Task task, CancellationTokenSource token) CreateReader(
            string topic,
            IRawMessageHandler rawHandler,
            TaskCompletionSource<bool> subscriptionTask,
            CancellationToken subscriptionCancellation)
        {
            var readerCreationLock = _readerCreationLocks.GetOrAdd(topic, new object());
            Monitor.Enter(readerCreationLock);
            try
            {
                if (_readers.TryGetValue((topic, rawHandler), out var tuple))
                {
                    if (tuple.task.IsCanceled) subscriptionTask.SetCanceled();
                    else if (tuple.task.IsCompleted) subscriptionTask.SetResult(true);
                    else if (tuple.task.IsFaulted) subscriptionTask.SetException(tuple.task.Exception);
                    
                    return tuple;
                }

                // Reader cancellation will let us stop this task on Dispose/Unsubscribe
                var readerCancellation = new CancellationTokenSource();

                var consumerTask = Task.Run(
                    async () => await ReaderTaskCreation(
                        topic,
                        rawHandler,
                        subscriptionTask,
                        subscriptionCancellation,
                        readerCancellation.Token),
                    // The Reader will handle the cancellation by gracefully shutting down.
                    CancellationToken.None);

                return (consumerTask, readerCancellation);
            }
            finally
            {
                Monitor.Exit(readerCreationLock);
                _readerCreationLocks.TryRemove(topic, out var _);
            }
        }

        private async Task ReaderTaskCreation(
            string topic,
            IRawMessageHandler rawHandler,
            TaskCompletionSource<bool> subscriptionTask,
            CancellationToken subscriptionCancellation,
            CancellationToken readerCancellation)
        {
            if (subscriptionCancellation.IsCancellationRequested)
            {
                subscriptionTask.SetCanceled();
                return;
            }

            IBlockingRawMessageReader<TOptions> reader;
            try
            {
                reader = _factory.Create(topic, _options);
            }
            catch (Exception e)
            {
                subscriptionTask.SetException(e);
                return;
            }

            subscriptionTask.SetResult(true);

            await ReadMessageLoop(topic, rawHandler, reader, _options, readerCancellation);
        }

        private static async Task ReadMessageLoop(
            string topic,
            IRawMessageHandler rawHandler,
            IBlockingRawMessageReader<TOptions> reader,
            TOptions options,
            CancellationToken consumerCancellation)
        {
            try
            {
                do
                {
                    // Blocking call to the reader to retrieve message
                    if (reader.TryGetMessage(out var msg, options))
                    {
                        await rawHandler.Handle(topic, msg, consumerCancellation);
                    }
                    else if (options.SleepBetweenPolling != default(TimeSpan))
                    {
                        // Implementations where TryGetMessage will block wait for a message
                        // there's no need to sleep here..
                        await Task.Delay(options.SleepBetweenPolling, consumerCancellation);
                    }

                    consumerCancellation.ThrowIfCancellationRequested();
                } while (true);
            }
            catch (OperationCanceledException oce)
            {
                options.ReaderStoppingCallback?.Invoke(topic, rawHandler, oce);
            }
            catch (Exception ex)
            {
                options.ErrorCallback?.Invoke(topic, rawHandler, ex);
            }
            finally
            {
                (reader as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Unsubscribes the specified raw handler from the topic
        /// </summary>
        /// <param name="topic">The topic to unsubscribe</param>
        /// <param name="rawHandler">The raw handle to unsubscribe</param>
        /// <param name="token">The unsubsctiption cancellation token</param>
        /// <remarks>
        /// Await to ensure continuation only happens once reader is disposed
        /// </remarks>
        /// <returns>
        /// A task that completes once the reader task is no longer executing
        /// </returns>
        public Task Unsubscribe(string topic, IRawMessageHandler rawHandler, CancellationToken token)
        {
            if (_readers.TryRemove((topic, rawHandler), out var tuple))
            {
                tuple.tokenSource.Cancel();
                tuple.tokenSource.Dispose();
            }

            var tcs = new TaskCompletionSource<bool>();
            // Unsubscribing is done once the reader has stopped
            tuple.task.ContinueWith(readerTask => tcs.SetResult(true), token);

            return tcs.Task;
        }

        public void Dispose()
        {
            foreach (var (topic, rawHandler) in _readers.Keys)
            {
                try
                {
                    Unsubscribe(topic, rawHandler, CancellationToken.None)
                        .GetAwaiter()
                        .GetResult();
                }
                catch // CA1065
                {
                    // ignored
                }
            }
        }
    }
}