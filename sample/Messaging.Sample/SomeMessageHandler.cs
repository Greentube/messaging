using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Sample
{
    // Class implementing IMessageHandler<T> to be invoked when T arrives
    internal class SomeMessageHandler : IMessageHandler<SomeMessage>
    {
        public Task Handle(SomeMessage message, CancellationToken _)
        {
            Console.WriteLine($"Handled: {message}.");
            return Task.CompletedTask;
        }
    }
}
