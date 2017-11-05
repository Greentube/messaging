using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public interface IRawMessageHandler
    {
        Task Handle(string topic, byte[] message, CancellationToken token);
    }
}