using System;
using System.Text;
using Xunit;

namespace Serialization.Xml.Tests
{
    public class XmlSerializerTests
    {
        [Fact]
        public void Serialize_FactoryDefined_FactoryIsCalled()
        {
            var sut = new XmlSerializer(new XmlOptions
            {
                Factory = _ => throw new DivideByZeroException()
            });
            Assert.Throws<DivideByZeroException>(() => sut.Serialize(new object()));
        }

        [Fact]
        public void Deserialize_FactoryDefined_FactoryIsCalled()
        {
            var sut = new XmlSerializer(new XmlOptions
            {
                Factory = _ => throw new DivideByZeroException()
            });
            Assert.Throws<DivideByZeroException>(() => sut.Deserialize(GetType(), new byte[0]));
        }

        [Fact]
        public void Serialize_NullObject_ThrowsNullArgument()
        {
            var sut = new XmlSerializer(new XmlOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Serialize((object) null));
        }

        [Fact]
        public void Deserialize_NullType_ThrowsNullArgument()
        {
            var sut = new XmlSerializer(new XmlOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(null, new byte[0]));
        }

        [Fact]
        public void Deserialize_NullBytes_ThrowsNullArgument()
        {
            var sut = new XmlSerializer(new XmlOptions());
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(GetType(), null));
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new XmlSerializer(null));
        }


        [Fact]
        public void Serialize_CustomNamespace()
        {
            var @namespace = Guid.NewGuid().ToString();

            var sut = new XmlSerializer(new XmlOptions { DefaultNamespace = @namespace});
            var bytes = sut.Serialize(new object());

            Assert.Contains(@namespace, Encoding.UTF8.GetString(bytes));
        }

        [Fact]
        public void Serialize_Deserialize()
        {
            // simple round-trip: here we're just testing XmlSerializer .. not much point

            var sut = new XmlSerializer(new XmlOptions());
            var expected = new TestClass {StringProperty = "string value"};
            var bytes = sut.Serialize(expected);
            var actual = sut.Deserialize(typeof(TestClass), bytes) as TestClass;

            Assert.NotNull(actual);
            Assert.NotSame(expected, actual);
            Assert.Equal(expected.StringProperty, actual.StringProperty);
        }

        // ReSharper disable once MemberCanBePrivate.Global - XmlSerializer doesn't like that
        public class TestClass
        {
            public string StringProperty { get; set; }
        }
    }
}