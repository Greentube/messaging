using System;
using Xunit;

namespace Serialization.Json.Tests
{
    public class JsonConvertSerializerTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonConvertSerializer(null));
        }
    }
}
