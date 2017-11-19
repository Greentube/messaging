using System;
using System.Runtime.ExceptionServices;

namespace Greentube.Messaging
{
    /// <summary>
    /// Options for a polling based reader
    /// </summary>
    /// <inheritdoc />
    public class PollingReaderOptions : IPollingOptions
    {
        /// <inheritdoc />
        public virtual TimeSpan SleepBetweenPolling { get; set; }
        /// <remarks>
        /// Allows access to the <see cref="T:Messaging.IRawMessageHandler" /> once the blocking read loop is exiting.
        /// It's invoked before disposing the reader
        /// </remarks>
        /// <inheritdoc />
        public Action<string, IRawMessageHandler, OperationCanceledException> ReaderStoppingCallback { get; set; }
        /// <remarks>
        /// By default it will rethrow the exception. It'll end the task and dispose the reader
        /// </remarks>
        /// <inheritdoc />
        public Action<string, IRawMessageHandler, Exception> ErrorCallback { get; set; } 
            = (topic, handler, ex) => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
