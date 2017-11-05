using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public interface IRawMessageHandlerSubscriber
    {
        Task Subscribe(string topic, IRawMessageHandler rawHandler, CancellationToken token);
    }
}