using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Messaging.Tests
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
    }
}
