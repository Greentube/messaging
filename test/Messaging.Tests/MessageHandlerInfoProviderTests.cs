using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NSubstitute;
using Xunit;
using static System.Threading.CancellationToken;

namespace Messaging.Tests
{
    public class MessageHandlerInfoProviderTests
    {
        [Theory, AutoSubstituteData]
        public async Task GetHandlerInfo_FindInnerClassHandlerInfo(
            string message,
            [Frozen] IMessageTypeTopicMap messageTypeTopicMap,
            MessageHandlerInfoProvider sut)
        {
            var messageType = typeof(string);
            messageTypeTopicMap.GetMessageTypes().Returns(new[] { messageType });

            var actual = sut.GetHandlerInfo();

            var handlerInfo = Assert.Single(actual);

            Assert.Equal(messageType, handlerInfo.messageType);
            Assert.Equal(typeof(IMessageHandler<string>), handlerInfo.handlerType);

            bool callbackFired = false;
            var handler = new TestStringHandler(m =>
            {
                callbackFired = true;
                Assert.Equal(message, m);
            });
            await handlerInfo.handleMethod(handler, message, None);
            Assert.True(callbackFired);
        }

        [Fact]
        public void Constructor_EmptyTypeTopicMap_ThrowsArgumentException()
        {
            var emptyMap = new MessageTypeTopicMap();

            var ex = Assert.Throws<ArgumentException>(() => new MessageHandlerInfoProvider(emptyMap));
            Assert.Equal($"{nameof(IMessageTypeTopicMap)} is empty.", ex.Message);
        }

        [Fact]
        public void Constructor_NullMessageTypeTopicMap_ThrowsNullArgument()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new MessageHandlerInfoProvider(null));
            Assert.Equal("typeTopicMap", ex.ParamName);
        }

        private class TestStringHandler : IMessageHandler<string>
        {
            private readonly Action<string> _callback;
            public TestStringHandler(Action<string> callback) => _callback = callback;

            public Task Handle(string message, CancellationToken token)
            {
                _callback(message);
                return Task.CompletedTask;
            }
        }
    }
}
