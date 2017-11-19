using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// Handles raw messages
    /// </summary>
    public interface IRawMessageHandler
    {
        /// <summary>
        /// Handles a serialized message from a specific topic
        /// </summary>
        /// <param name="topic">The topic which this message arrived</param>
        /// <param name="message">The serialized message</param>
        /// <param name="token">A cancellation token</param>
        /// <returns></returns>
        Task Handle(string topic, byte[] message, CancellationToken token);
    }
}