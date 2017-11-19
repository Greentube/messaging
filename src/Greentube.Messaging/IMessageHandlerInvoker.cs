using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// Message handler invoker
    /// </summary>
    public interface IMessageHandlerInvoker
    {
        /// <summary>
        /// Invokes a <see cref="IMessageHandler"/> for the specified message based on its type
        /// </summary>
        /// <param name="message">The message to invoke the handler with</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task Invoke(object message, CancellationToken token);
    }
}