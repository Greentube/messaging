using System;
using System.Collections.Generic;

namespace Messaging
{
    /// <summary>
    /// Map of message type and correspondent topic
    /// </summary>
    public interface IMessageTypeTopicMap : IReadOnlyCollection<KeyValuePair<Type, string>>
    {
        /// <summary>
        /// Add a map of message type and topic
        /// </summary>
        /// <param name="type">Type of message</param>
        /// <param name="topic">Topic</param>
        void Add(Type type, string topic);
        /// <summary>
        /// Remove a map by the message type
        /// </summary>
        /// <param name="type"></param>
        void Remove(Type type);
        /// <summary>
        /// Get the topic of a corresponding message type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string Get(Type type);
        /// <summary>
        /// Get the message type of a corresponding topic
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Type Get(string topic);
        /// <summary>
        /// Get all topics
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetTopics();
        /// <summary>
        /// Get all message types
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetMessageTypes();
    }
}