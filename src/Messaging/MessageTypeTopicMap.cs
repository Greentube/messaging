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
        void Remove(Type type);

        string Get(Type type);
        Type Get(string topic);
        IEnumerable<string> GetTopics();
    }

    /// <inheritdoc cref="IMessageTypeTopicMap"/>
    public class MessageTypeTopicMap : IMessageTypeTopicMap
    {
        private readonly ConcurrentDictionary<Type, string> _messageTopicMap =
            new ConcurrentDictionary<Type, string>();
        private readonly ConcurrentDictionary<string, Type> _topicMessageMap =
            new ConcurrentDictionary<string, Type>();

        public int Count => _messageTopicMap.Count;

        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator() => _messageTopicMap.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(Type type, string topic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (topic == null) throw new ArgumentNullException(nameof(topic));

            if (_messageTopicMap.TryAdd(type, topic))
            {
                _topicMessageMap.TryAdd(topic, type);
            }
        }

        public string Get(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            _messageTopicMap.TryGetValue(type, out var topic);
            return topic;
        }

        public Type Get(string topic)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));

            _topicMessageMap.TryGetValue(topic, out var type);
            return type;
        }

        public IEnumerable<string> GetTopics()
        {
            return _messageTopicMap.Values;
        }

        public void Remove(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (_messageTopicMap.TryRemove(type, out var topic))
            {
                _topicMessageMap.TryRemove(topic, out var _);
            }
        }
    }
}