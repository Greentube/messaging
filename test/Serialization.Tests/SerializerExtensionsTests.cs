using System;
using Xunit;

namespace Serialization.Tests
{
    public class SerializerExtensionsTests
    {
        [Fact]
        public void Deserialize_ThrowsOnNullData()
        {
            Assert.Throws<ArgumentNullException>(() => SerializerExtensions.Deserialize<object>(null, null));
        }
    }
}
