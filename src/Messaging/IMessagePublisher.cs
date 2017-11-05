using System.Threading.Tasks;

namespace Messaging
{
    /// <summary>
    /// Message Publisher
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a message of type <typeparam name="TMessage"></typeparam>
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        Task Publish<TMessage>(TMessage message);
    }

}