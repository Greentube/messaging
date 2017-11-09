using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Sample.Kafka
{
    // Will be discovered (auto-registered)
    public class SomeOtherMessageHandler : IMessageHandler<SomeMessage>
    {
        public Task Handle(SomeMessage message, CancellationToken _)
        {
            Console.WriteLine($"Other handler Handled: {message.Body}.");
            return Task.CompletedTask;
        }
    }
}