using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// Subscribes raw message handlers to topics
    /// </summary>
    public interface IRawMessageHandlerSubscriber
    {
        /// <summary>
        /// Subscribes to the specified topic with the provided <see cref="IRawMessageHandler"/>
        /// </summary>
        /// <param name="topic">To topic to subscribe to</param>
        /// <param name="rawHandler">The handler to invoke</param>
        /// <param name="token">A cancellation token</param>
        /// <returns></returns>
        Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken token);
    }
}