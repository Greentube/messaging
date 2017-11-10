using System;
using ProtoBuf;
using Xunit;

namespace Serialization.ProtoBuf.Tests
{
    public class ProtoBufSerializerTests
    {
        [Fact]
        public void Serialize_NullObject_ThrowsNullArgument()
        {
            var sut = new ProtoBufSerializer(new ProtoBufOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Serialize((object) null));
        }

        [Fact]
        public void Deserialize_NullType_ThrowsNullArgument()
        {
            var sut = new ProtoBufSerializer(new ProtoBufOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(null, new byte[0]));
        }

        [Fact]
        public void Deserialize_NullBytes_ThrowsNullArgument()
        {
            var sut = new ProtoBufSerializer(new ProtoBufOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(GetType(), null));
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProtoBufSerializer(null));
        }

        [Fact]
        public void Serialize_Deserialize()
        {
            // simple round-trip: here we're just testing ProtoBufSerializer .. not much point

            var sut = new ProtoBufSerializer(new ProtoBufOptions());
            var expected = new TestClass {StringProperty = "string value"};
            var bytes = sut.Serialize(expected);
            var actual = sut.Deserialize(typeof(TestClass), bytes) as TestClass;

            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.StringProperty, actual.StringProperty);
        }

        // ReSharper disable once MemberCanBePrivate.Global - ProtoBufSerializer doesn't like that
        [ProtoContract]
        public class TestClass
        {
            [ProtoMember(1)]
            public string StringProperty { get; set; }
        }
    }
}