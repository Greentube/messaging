using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging.Sample
{
    // Class implementing IMessageHandler<T> to be invoked when T arrives
    public class SomeMessageHandler : IMessageHandler<SomeMessage>
    {
        public Task Handle(SomeMessage message, CancellationToken _)
        {
            Console.WriteLine($"Handled: {message.Body}.");
            return Task.CompletedTask;
        }
    }
}
