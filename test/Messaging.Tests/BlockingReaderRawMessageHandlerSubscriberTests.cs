using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using static System.Threading.CancellationToken;

namespace Messaging.Tests
{
    public class BlockingReaderRawMessageHandlerSubscriberTests
    {
        [Theory, AutoSubstituteData]
        public async Task Subscribe_CallbackRethrows_UnsubscribeDoesNotFailAwait(
            string topic,
            IDisposableBlockingRawMessageReader<IPollingOptions> reader,
            [Frozen] IPollingOptions options,
            [Frozen] IRawMessageHandler rawMessageHandler,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            options.ReaderStoppingCallback = (s, handler, ex) => throw ex;
            reader.TryGetMessage(out var _, options).Throws<DivideByZeroException>();

            // Act
            await sut.Subscribe(topic, rawMessageHandler, None);
            // await doesn't throw on OperationCancelledException
            await sut.Unsubscribe(topic, rawMessageHandler, None);
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_AfterUnsubscribing_CreatesNewReader(
            string topic,
            IDisposableBlockingRawMessageReader<IPollingOptions> reader,
            [Frozen] IPollingOptions options,
            [Frozen] IRawMessageHandler rawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            factory.Create(topic, options).Returns(reader);

            var disposeEvent = new ManualResetEventSlim();
            reader.When(r => r.Dispose()).Do(r => disposeEvent.Set());

            // Act
            await sut.Subscribe(topic, rawMessageHandler, None);
            await sut.Unsubscribe(topic, rawMessageHandler, None);
            Assert.True(disposeEvent.Wait(1000)); // wait task complete
            await sut.Subscribe(topic, rawMessageHandler, None);

            // Assert
            factory.Received(2).Create(topic, options);
        }

        [Theory, AutoSubstituteData]
        public async Task Unsubscribe_CallbackInvoked(
            string topic,
            IDisposableBlockingRawMessageReader<IPollingOptions> reader,
            [Frozen] IPollingOptions options,
            [Frozen] IRawMessageHandler rawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            var callbackInvokedEvent = new ManualResetEventSlim();
            options.ReaderStoppingCallback = (t, handler, arg3) => callbackInvokedEvent.Set();
            factory.Create(topic, options).Returns(reader);

            // Act
            await sut.Subscribe(topic, rawMessageHandler, None);
            await sut.Unsubscribe(topic, rawMessageHandler, None);

            // Assert
            Assert.True(callbackInvokedEvent.Wait(1000));
        }

        [Theory, AutoSubstituteData]
        public async Task Unsubscribe_DisposesReader(
            string topic,
            IDisposableBlockingRawMessageReader<IPollingOptions> reader,
            [Frozen] IPollingOptions options,
            [Frozen] IRawMessageHandler rawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            factory.Create(topic, options).Returns(reader);

            var disposeEvent = new ManualResetEventSlim();
            reader.When(r => r.Dispose()).Do(r => disposeEvent.Set());

            // Act
            await sut.Subscribe(topic, rawMessageHandler, None);
            await sut.Unsubscribe(topic, rawMessageHandler, None);

            // Assert
            Assert.True(disposeEvent.Wait(1000));
        }

        [Theory, AutoSubstituteData]
        public async Task Unsubscribe_DifferentHandlers_DisposesSingleReader(
            string topic,
            IDisposableBlockingRawMessageReader<IPollingOptions> firstReader,
            IDisposableBlockingRawMessageReader<IPollingOptions> secondReader,
            [Frozen] IPollingOptions options,
            IRawMessageHandler firstRawMessageHandler,
            IRawMessageHandler secondRawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            factory.Create(topic, options).Returns(firstReader, secondReader);

            var disposeEvent = new ManualResetEventSlim();
            firstReader.When(r => r.Dispose()).Do(r => disposeEvent.Set());

            // Act
            await sut.Subscribe(topic, firstRawMessageHandler, None);
            await sut.Subscribe(topic, secondRawMessageHandler, None);
            await sut.Unsubscribe(topic, firstRawMessageHandler, None);

            // Assert
            Assert.True(disposeEvent.Wait(1000));
            secondReader.DidNotReceive().Dispose();
        }

        [Theory, AutoSubstituteData]
        public async Task Unsubscribe_DifferentTopics_DisposesSingleReader(
            string firstTopic,
            string secondTopic,
            IDisposableBlockingRawMessageReader<IPollingOptions> firstReader,
            IDisposableBlockingRawMessageReader<IPollingOptions> secondReader,
            [Frozen] IPollingOptions options,
            IRawMessageHandler firstRawMessageHandler,
            IRawMessageHandler secondRawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            factory.Create(firstTopic, options).Returns(firstReader);
            factory.Create(secondTopic, options).Returns(secondReader);

            var disposeEvent = new ManualResetEventSlim();
            firstReader.When(r => r.Dispose()).Do(r => disposeEvent.Set());

            // Act
            await sut.Subscribe(firstTopic, firstRawMessageHandler, None);
            await sut.Subscribe(secondTopic, secondRawMessageHandler, None);
            await sut.Unsubscribe(firstTopic, firstRawMessageHandler, None);

            // Assert
            Assert.True(disposeEvent.Wait(1000));
            secondReader.DidNotReceive().Dispose();
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_HandlerThrows_DisposesReader(
            string topic,
            byte[] message,
            IDisposableBlockingRawMessageReader<IPollingOptions> reader,
            [Frozen] IPollingOptions options,
            [Frozen] IRawMessageHandler rawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            factory.Create(topic, options).Returns(reader);

            var disposeEvent = new ManualResetEventSlim();
            reader.When(r => r.Dispose()).Do(r => disposeEvent.Set());

            reader.TryGetMessage(out var _, options)
                .Returns(r =>
                {
                    r[0] = message;
                    return true;
                });

            rawMessageHandler.Handle(topic, message, Arg.Any<CancellationToken>())
                .Throws<ArgumentNullException>();

            // Act
            await sut.Subscribe(topic, rawMessageHandler, None);

            // Assert
            Assert.True(disposeEvent.Wait(1000));
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_ReaderThrows_DisposesReader(
            string topic,
            IRawMessageHandler rawMessageHandler,
            IDisposableBlockingRawMessageReader<IPollingOptions> reader,
            [Frozen] IPollingOptions options,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            factory.Create(topic, options).Returns(reader);

            var disposeEvent = new ManualResetEventSlim();
            reader.When(r => r.Dispose()).Do(r => disposeEvent.Set());

            reader.TryGetMessage(out var _, options)
                .Throws<ArgumentNullException>();

            await sut.Subscribe(topic, rawMessageHandler, None);

            Assert.True(disposeEvent.Wait(1000));
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_ReaderFactoryThrows_SubscriptionFails(
            string topic,
            IRawMessageHandler rawMessageHandler,
            [Frozen] IPollingOptions options,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            factory.Create(topic, options).Throws(new DivideByZeroException());
            await Assert.ThrowsAsync<DivideByZeroException>(() => sut.Subscribe(topic, rawMessageHandler, None));
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_MultipleCallsDifferentRawHandlers_CreatesSingleReader(
            string topic,
            IRawMessageHandler firstRawMessageHandler,
            IRawMessageHandler secondRawMessageHandler,
            [Frozen] IPollingOptions options,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            await Task.WhenAll(Subscrioptions());

            factory.Received(2).Create(topic, options);

            IEnumerable<Task> Subscrioptions()
            {
                for (int i = 0; i < 5; i++)
                {
                    yield return sut.Subscribe(topic, firstRawMessageHandler, None);
                    yield return sut.Subscribe(topic, secondRawMessageHandler, None);
                }
            }
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_MultipleCallsSameRawHandler_CreatesSingleReader(
            string topic,
            IRawMessageHandler rawMessageHandler,
            [Frozen] IPollingOptions options,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            var tasks = Enumerable.Range(1, 5).Select(_ => sut.Subscribe(topic, rawMessageHandler, None));
            await Task.WhenAll(tasks);

            factory.Received(1).Create(topic, options);
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_CancelledToken_CancelsSubscriptionTask(
            string topic,
            IRawMessageHandler rawMessageHandler,
            [Frozen] IPollingOptions options,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var task = sut.Subscribe(topic, rawMessageHandler, cts.Token);

            // Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => task);

            factory.DidNotReceiveWithAnyArgs().Create(topic, options);
        }

        [Theory, AutoSubstituteData]
        public async Task Dispose_DisposesAllReaders(
            string topic,
            IDisposableBlockingRawMessageReader<IPollingOptions> firstReader,
            IDisposableBlockingRawMessageReader<IPollingOptions> secondReader,
            [Frozen] IPollingOptions options,
            IRawMessageHandler firstRawMessageHandler,
            IRawMessageHandler secondRawMessageHandler,
            [Frozen] IBlockingRawMessageReaderFactory<IPollingOptions> factory,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            // Arrange
            factory.Create(topic, options).Returns(firstReader, secondReader);

            var firstDisposeEvent = new ManualResetEventSlim();
            firstReader.When(r => r.Dispose()).Do(r => firstDisposeEvent.Set());
            var secondDisposeEvent = new ManualResetEventSlim();
            firstReader.When(r => r.Dispose()).Do(r => secondDisposeEvent.Set());

            await sut.Subscribe(topic, firstRawMessageHandler, None);
            await sut.Subscribe(topic, secondRawMessageHandler, None);

            // Act
            sut.Dispose();

            // Assert
            Assert.True(firstDisposeEvent.Wait(1000));
            Assert.True(secondDisposeEvent.Wait(1000));
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_NullRawHandler_ThrowsArgumentNull(
            string topic,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Subscribe(topic, null, None));
            Assert.Equal("rawHandler", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public async Task Subscribe_NullTopic_ThrowsArgumentNull(
            IRawMessageHandler rawMessageHandler,
            BlockingReaderRawMessageHandlerSubscriber<IPollingOptions> sut)
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Subscribe(null, rawMessageHandler, None));
            Assert.Equal("topic", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullOptions_ThrowsArgumentNull(IBlockingRawMessageReaderFactory<IPollingOptions> factory)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new BlockingReaderRawMessageHandlerSubscriber<IPollingOptions>(factory, null));
            Assert.Equal("options", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NullFactory_ThrowsArgumentNull(IPollingOptions options)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new BlockingReaderRawMessageHandlerSubscriber<IPollingOptions>(null, options));
            Assert.Equal("factory", ex.ParamName);
        }
    }
}