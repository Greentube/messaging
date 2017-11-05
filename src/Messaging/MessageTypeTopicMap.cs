using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Messaging
{
    /// <summary>
    /// Map of message type and correspondent topic
    /// </summary>
    public interface IMessageTypeTopicMap : IReadOnlyCollection<KeyValuePair<Type, string>>
    {
        void Add(Type type, string topic);
        string Get(Type type);
        void Remove(Type type);
    }

    /// <inheritdoc cref="IMessageTypeTopicMap"/>
    public class MessageTypeTopicMap : IMessageTypeTopicMap
    {
        private readonly ConcurrentDictionary<Type, string> _map =
            new ConcurrentDictionary<Type, string>();

        public void Add(Type type, string topic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (topic == null) throw new ArgumentNullException(nameof(topic));

            _map.TryAdd(type, topic);
        }

        public string Get(Type type)
        {
            _map.TryGetValue(type, out var topic);
            return topic;
        }

        public void Remove(Type type)
        {
            _map.TryRemove(type, out var _);
        }

        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator() => _map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _map.Count;
    }
}