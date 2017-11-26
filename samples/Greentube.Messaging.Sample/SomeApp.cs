using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Greentube.Messaging.Sample
{
    public class SomeApp : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public SomeApp(Action<IServiceCollection> servicesAction)
        {
            // ConfigureServices()
            var services = new ServiceCollection();

            // Sample specific setup: .AddMessaging()
            servicesAction(services);

            services.AddOptions();

            _serviceProvider = services.BuildServiceProvider();

            // Configure()
            // app.AddMessagingSubscriptions()
            var map = _serviceProvider.GetRequiredService<IMessageTypeTopicMap>();
            var rawSubscriber = _serviceProvider.GetRequiredService<IRawMessageHandlerSubscriber>();
            var rawHandler = _serviceProvider.GetRequiredService<IRawMessageHandler>();
            foreach (var topic in map.GetTopics())
            {
                rawSubscriber.Subscribe(topic, rawHandler, CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public IMessagePublisher Run()
        {
            var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();

            Console.WriteLine("Publishing 'SomeMessage'...");
            publisher.Publish(new SomeMessage { Body = $"{DateTime.Now} Some message body: {Guid.NewGuid()}" }, CancellationToken.None)
                .GetAwaiter()
                .GetResult();

            return publisher;
        }

        public void Dispose()
        {
            Console.WriteLine("Running... Press any key to quit.");
            Console.ReadKey();

            _serviceProvider.Dispose();
        }
    }
}
