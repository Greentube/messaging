using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// Handles a message of type <typeparam name="TMessage"></typeparam>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <inheritdoc />
    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        /// <summary>
        /// Handles <typeparam name="TMessage"></typeparam>
        /// </summary>
        /// <param name="message">The message to be handled</param>
        /// <param name="token">A cancellation token</param>
        /// <returns></returns>
        Task Handle(TMessage message, CancellationToken token);
    }

    /// <summary>
    /// Message handler
    /// </summary>
    /// <remarks>Marker interface</remarks>
    public interface IMessageHandler
    {

    }
}