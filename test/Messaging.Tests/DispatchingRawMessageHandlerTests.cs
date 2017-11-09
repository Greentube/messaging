using System;
using Xunit;

namespace Messaging.Tests
{
    public class DispatchingRawMessageHandlerTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new DispatchingRawMessageHandler(null, null, null));
        }
    }
}
