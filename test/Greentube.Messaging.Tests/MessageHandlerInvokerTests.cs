using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
using Xunit;
using static System.Threading.CancellationToken;

namespace Greentube.Messaging.Tests
{
    public class MessageHandlerInvokerTests
    {
        [Theory, AutoSubstituteData]
        public async Task Invoke_CallsHandlerInstance(
            [Frozen] IMessageHandlerInfoProvider messageHandlerInfoProvider,
            IMessageHandler<Guid> messageHandler,
            Guid message,
            Fixture fixture)
        {
            // Arrange
            var callbackFired = false;
            IMessageHandler<Guid> handlerInvoked = null;
            Guid messageInvoked;

            Task Callback(object hi, object mi, CancellationToken _)
            {
                callbackFired = true;
                messageInvoked = (Guid) mi;
                handlerInvoked = hi as IMessageHandler<Guid>;
                return Task.CompletedTask;
            }

            messageHandlerInfoProvider.GetHandlerInfo()
                .Returns(new[]
                {
                    (typeof(Guid), typeof(IMessageHandler<Guid>),
                    (Func<object, object, CancellationToken, Task>) Callback)
                });

            var handlerFactory = new Func<Type, object>(t => messageHandler);

            fixture.Register(handlerFactory);
            var sut = fixture.Create<MessageHandlerInvoker>();

            // Act
            await sut.Invoke(message, None);

            // Assert
            messageHandlerInfoProvider.Received(1).GetHandlerInfo();
            Assert.True(callbackFired);
            Assert.Equal(message, messageInvoked);
            Assert.Equal(messageHandler, handlerInvoked);
        }

        [Theory, AutoSubstituteData]
        public async Task Invoke_FactoryReturnsNull_ThrowsInvalidOperation(
            [Frozen] IMessageHandlerInfoProvider messageHandlerInfoProvider,
            IMessageHandler<bool> messageHandler,
            string message,
            Fixture fixture)
        {
            Task Callback(object hi, object mi, CancellationToken _) => Task.CompletedTask;
            messageHandlerInfoProvider.GetHandlerInfo()
                .Returns(new[]
                {
                    (message.GetType(), typeof(IMessageHandler<string>),
                    (Func<object, object, CancellationToken, Task>) Callback)
                });

            var handlerFactory = new Func<Type, object>(t => null);

            fixture.Register(handlerFactory);
            var sut = fixture.Create<MessageHandlerInvoker>();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Invoke(message, None));
            Assert.Equal($"Message of type {message.GetType()} yielded no handler from factory.", ex.Message);
        }

        [Theory, AutoSubstituteData]
        public async Task Invoke_NoHandlerForMessage_ThrowsInvalidOperation(
            [Frozen] IMessageHandlerInfoProvider messageHandlerInfoProvider,
            IMessageHandler<bool> messageHandler,
            string message,
            Fixture fixture)
        {
            Task Callback(object hi, object mi, CancellationToken _) => Task.CompletedTask;
            messageHandlerInfoProvider.GetHandlerInfo()
                .Returns(new[]
                {
                    (typeof(bool), typeof(IMessageHandler<bool>),
                    (Func<object, object, CancellationToken, Task>) Callback)
                });

            var handlerFactory = new Func<Type, object>(t => messageHandler);

            fixture.Register(handlerFactory);
            var sut = fixture.Create<MessageHandlerInvoker>();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Invoke(message, None));
            Assert.Equal($"No message handler found for message type {message.GetType()}.", ex.Message);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NoHandlerInfo_ThrowsInvalidOperation(
            IMessageHandlerInfoProvider messageHandlerInfoProvider)
        {
            messageHandlerInfoProvider.GetHandlerInfo()
                .Returns(new (
                    Type messageType,
                    Type handlerType,
                    Func<object, object, CancellationToken, Task> handleMethod)[0]);

            var ex = Assert.Throws<InvalidOperationException>(
                () => new MessageHandlerInvoker(messageHandlerInfoProvider, type => new object()));

            Assert.Equal(
                $"{nameof(IMessageHandlerInfoProvider)}.{nameof(IMessageHandlerInfoProvider.GetHandlerInfo)} " +
                "hasn't resolved any handler information.", ex.Message);
        }

        [Fact]
        public void Constructor_NoHandlerInfoProvider_ThrowsArgumentNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new MessageHandlerInvoker(null, type => new object()));
            Assert.Equal("messageHandlerInfoProvider", ex.ParamName);
        }

        [Theory, AutoSubstituteData]
        public void Constructor_NoHandlerFactory_ThrowsArgumentNull(
            IMessageHandlerInfoProvider messageHandlerInfoProvider)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new MessageHandlerInvoker(messageHandlerInfoProvider, null));
            Assert.Equal("handlerFactory", ex.ParamName);
        }
    }
}