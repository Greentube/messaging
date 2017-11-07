using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Sample
{
    public class SomeApp : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public SomeApp(Action<ServiceCollection> servicesAction)
        {
            // ConfigureServices()
            var services = new ServiceCollection();

            // Sample specific setup: .AddMessaging()
            servicesAction(services);

            // options.MessageTypeTopicMap.Add(type,topic);
            // - auto discovery based on SomeMessage attributes?
            services.AddSingleton<IMessageTypeTopicMap>(c =>
                new MessageTypeTopicMap
                {
                    {typeof(SomeMessage), "SomeTopic"}
                });

            services.AddScoped<IMessageHandler<SomeMessage>, SomeMessageHandler>();
            services.AddScoped<SomeMessageHandler, SomeMessageHandler>();

            services.AddOptions();

            _serviceProvider = services.BuildServiceProvider();

            // Configure()
            // app.AddSubscriptions()
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

        public void Run()
        {
            var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();

            Console.WriteLine("Publishing 'SomeMessage'...");
            publisher.Publish(new SomeMessage { Body = "Some message body." }, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}
