using System;
using NSubstitute;
using Xunit;

namespace Greentube.Serialization.Tests
{
    public class SerializerExtensionsTests
    {
        [Fact]
        public void Deserialize_ThrowsOnNullData()
        {
            Assert.Throws<ArgumentNullException>(() => SerializerExtensions.Deserialize<object>(null, null));
        }

        [Fact]
        public void Deserialize_TypeOfPassedAlongWithBytes()
        {
            var serializerMock = Substitute.For<ISerializer>();
            var expectedBytes = new byte[] {1};
            serializerMock.Deserialize<SerializerExtensionsTests>(expectedBytes);
            serializerMock.Received().Deserialize(typeof(SerializerExtensionsTests), expectedBytes);
        }
    }
}
