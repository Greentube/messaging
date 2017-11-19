using System;
using MessagePack;
using Xunit;

namespace Greentube.Serialization.MessagePack.Tests
{
    public class MessagePackSerializerTests
    {
        [Fact]
        public void Serialize_NullObject_ThrowsNullArgument()
        {
            var sut = new MessagePackSerializer(new MessagePackOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Serialize((object) null));
        }

        [Fact]
        public void Deserialize_NullType_ThrowsNullArgument()
        {
            var sut = new MessagePackSerializer(new MessagePackOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(null, new byte[0]));
        }

        [Fact]
        public void Deserialize_NullBytes_ThrowsNullArgument()
        {
            var sut = new MessagePackSerializer(new MessagePackOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(GetType(), null));
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MessagePackSerializer(null));
        }

        [Fact]
        public void Serialize_Deserialize()
        {
            // simple round-trip: here we're just testing MessagePackSerializer .. not much point

            var sut = new MessagePackSerializer(new MessagePackOptions());
            var expected = new TestClass {StringProperty = "string value"};
            var bytes = sut.Serialize(expected);
            var actual = sut.Deserialize(typeof(TestClass), bytes) as TestClass;

            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.StringProperty, actual.StringProperty);
        }

        [Fact]
        public void Serialize_Lz4Compressed_Deserialize()
        {
            var sut = new MessagePackSerializer(new MessagePackOptions{UseLz4Compression = true});
            var expected = new TestClass {StringProperty = "string value"};
            var bytes = sut.Serialize(expected);
            var actual = sut.Deserialize(typeof(TestClass), bytes) as TestClass;

            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.StringProperty, actual.StringProperty);
        }

        [Fact]
        public void Serialize_ContractlessStandardResolver_Deserialize()
        {
            var sut = new MessagePackSerializer(new MessagePackOptions
            {
                FormatterResolver = global::MessagePack.Resolvers.ContractlessStandardResolver.Instance
            });
            var expected = new TestClassNoAttributes {StringProperty = "string value"};
            var bytes = sut.Serialize(expected);
            var actual = sut.Deserialize(expected.GetType(), bytes) as TestClassNoAttributes;

            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.StringProperty, actual.StringProperty);
        }

        // ReSharper disable once MemberCanBePrivate.Global - MessagePackSerializer doesn't like that
        [MessagePackObject]
        public class TestClass
        {
            [Key(0)]
            public string StringProperty { get; set; }
        }

        // ReSharper disable once MemberCanBePrivate.Global - MessagePackSerializer doesn't like that
        public class TestClassNoAttributes
        {
            public string StringProperty { get; set; }
        }
    }
}
