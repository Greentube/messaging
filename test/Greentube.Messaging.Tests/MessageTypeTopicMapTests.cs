using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Greentube.Messaging.Tests
{
    public class MessageTypeTopicMapTests
    {
        private readonly MessageTypeTopicMap _sut = new MessageTypeTopicMap();

        [Fact]
        public void Add_RetrievableByTopic()
        {
            _sut.Add(GetType(), GetType().FullName);
            Assert.Equal(GetType(), _sut.Get(GetType().FullName));
        }

        [Fact]
        public void Add_RetrievableByType()
        {
            _sut.Add(GetType(), GetType().FullName);
            Assert.Equal(GetType().FullName, _sut.Get(GetType()));
        }

        [Fact]
        public void Add_RetrievableByGetTopics()
        {
            _sut.Add(GetType(), GetType().FullName);
            Assert.Equal(GetType().FullName, _sut.GetTopics().Single());
        }

        [Fact]
        public void Add_RetrievableByGetMessageTypes()
        {
            _sut.Add(GetType(), GetType().FullName);
            Assert.Equal(GetType(), _sut.GetMessageTypes().Single());
        }

        [Fact]
        public void Add_NullTopic_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Add(GetType(), null));
        }

        [Fact]
        public void Add_NullType_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Add(null, GetType().FullName));
        }

        [Fact]
        public void Get_NullType_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Get((Type)null));
        }

        [Fact]
        public void Get_NullTopic_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Get((string)null));
        }

        [Fact]
        public void Add_NoDuplicates()
        {
            _sut.Add(GetType(), GetType().FullName);
            _sut.Add(GetType(), GetType().FullName);
            Assert.Single(_sut);
        }

        [Fact]
        public void Remove_UnknownType_NoOp()
        {
            _sut.Remove(GetType());
            Assert.Empty(_sut);
        }

        [Fact]
        public void Remove_RemovesOnlySpecifiedType()
        {
            _sut.Add(GetType(), GetType().FullName);
            _sut.Add(typeof(Assert), typeof(Assert).FullName);

            _sut.Remove(GetType());

            Assert.Single(_sut);
            Assert.Equal(typeof(Assert), _sut.GetMessageTypes().Single());
            Assert.Equal(typeof(Assert).FullName, _sut.GetTopics().Single());
        }

        [Fact]
        public void Remove_NullType_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Remove(null));
        }

        [Fact]
        public void GetEnumerator_AsEnumerator()
        {
            _sut.Add(GetType(), GetType().FullName);
            IEnumerable enuSut = _sut;
            var enumerator = enuSut.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<Type, string>(GetType(), GetType().FullName), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

#pragma warning disable xUnit2013 // Do not use equality check to check for collection size. - Let me test a call to Count?
        [Fact]
        public void Count_StartsAtZero() => Assert.Equal(0, _sut.Count);

        [Fact]
        public void Count_ReflectsAddCount()
        {
            _sut.Add(GetType(), GetType().FullName);
            Assert.Equal(1, _sut.Count);
        }
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
    }
}
