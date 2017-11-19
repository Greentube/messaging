using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging.Sample.Kafka
{
    // Will be discovered (auto-registered)
    internal class SomeOtherMessageHandler : IMessageHandler<SomeOtherMessage>
    {
        public Task Handle(SomeOtherMessage message, CancellationToken _)
        {
            Console.WriteLine($"Some other handler Handled SomeOtherMessage: #{message.Number}");
            return Task.CompletedTask;
        }
    }
}