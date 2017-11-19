using System;
using System.Text;
using Xunit;

namespace Greentube.Serialization.Json.Tests
{
    public class JsonConvertSerializerTests
    {
       [Fact]
        public void Serialize_EncodingDefined()
        {
            var sut = new JsonSerializer(new JsonOptions { Encoding = Encoding.ASCII });

            var actualBytes = sut.Serialize("Não suporta!");

            var actual = Encoding.UTF8.GetString(actualBytes);
            Assert.Equal("\"N?o suporta!\"", actual);
        }

        [Fact]
        public void Serialize_NullObject_ThrowsNullArgument()
        {
            var sut = new JsonSerializer(new JsonOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Serialize((object) null));
        }

        [Fact]
        public void Deserialize_NullType_ThrowsNullArgument()
        {
            var sut = new JsonSerializer(new JsonOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(null, new byte[0]));
        }

        [Fact]
        public void Deserialize_NullBytes_ThrowsNullArgument()
        {
            var sut = new JsonSerializer(new JsonOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(GetType(), null));
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonSerializer(null));
        }

        [Fact]
        public void Serialize_Deserialize()
        {
            // simple round-trip: here we're just testing JsonSerializer .. not much point

            var sut = new JsonSerializer(new JsonOptions());
            var expected = new TestClass {StringProperty = "string value"};
            var bytes = sut.Serialize(expected);
            var actual = sut.Deserialize(typeof(TestClass), bytes) as TestClass;

            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.StringProperty, actual.StringProperty);
        }

        private class TestClass
        {
            public string StringProperty { get; set; }
        }
    }
}
