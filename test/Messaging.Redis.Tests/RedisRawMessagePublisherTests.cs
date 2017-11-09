using System;
using Xunit;

namespace Messaging.Redis.Tests
{
    public class RedisRawMessagePublisherTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullArgument()
        {
            Assert.Throws<ArgumentException>(() => new RedisRawMessagePublisher(null));
        }
    }
}
