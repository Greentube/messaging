using System.Threading.Tasks;

namespace Messaging
{
    /// <summary>
    /// Handles a message of type <typeparam name="TMessage"></typeparam>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        /// <summary>
        /// Handles <typeparam name="TMessage"></typeparam>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Handle(TMessage message);
    }

    /// <summary>
    /// Message handler
    /// </summary>
    /// <remarks>Marker interface</remarks>
    public interface IMessageHandler
    {

    }
}