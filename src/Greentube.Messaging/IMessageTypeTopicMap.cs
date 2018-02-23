using System;
using System.Collections.Generic;

namespace Greentube.Messaging
{
    /// <summary>
    /// Map of message type and correspondent topic
    /// </summary>
    /// <inheritdoc />
    public interface IMessageTypeTopicMap
    {
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