using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// Message Publisher
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a message of type <typeparam name="TMessage"></typeparam>
        /// </summary>
        /// <param name="message">The message to be published</param>
        /// <param name="token">A cancellation token</param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        Task Publish<TMessage>(TMessage message, CancellationToken token);
    }

}