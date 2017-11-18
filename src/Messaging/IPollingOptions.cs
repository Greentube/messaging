using System;

namespace Messaging
{
    /// <summary>
    /// Options for a polling based message readers
    /// </summary>
    public interface IPollingOptions
    {
        /// <summary>
        /// Time to wait between calls to Read
        /// </summary>
        TimeSpan SleepBetweenPolling { get; set; }
        /// <summary>
        /// Invoked when the reader task is requested to stop.
        /// </summary>
        Action<string, IRawMessageHandler, OperationCanceledException> ReaderStoppingCallback { get; set; }
        /// <summary>
        /// Invoked when an unhandled exception happened while reading messages from a topic
        /// </summary>
        Action<string, IRawMessageHandler, Exception> ErrorCallback { get; set; } 
    }
}