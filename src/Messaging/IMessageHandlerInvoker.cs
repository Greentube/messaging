using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public interface IMessageHandlerInvoker
    {
        Task Invoke(object message, CancellationToken token);
    }
}