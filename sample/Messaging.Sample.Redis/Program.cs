using System;
using System.Threading;
using System.Threading.Tasks;
using Messaging.Redis;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Meta;
using Serialization;
using Serialization.Protobuf;
using StackExchange.Redis;
using static System.Threading.CancellationToken;

namespace Messaging.Sample.Redis
{
    // Message, POCO class
    public class SomeMessage
    {
        public string Body { get; set; }
    }

    // Class implementing IMessageHandler<T> to be invoked when T arrives
    internal class SomeMessageHandler : IMessageHandler<SomeMessage>
    {
        public Task Handle(SomeMessage message, CancellationToken _)
        {
            Console.WriteLine($"Handled: {message}.");
            return Task.CompletedTask;
        }
    }

    public class Program
    {
        static void Main()
        {
            // ConfigureServices()
            var services = new ServiceCollection();

            // services.AddMessaging(options =>);
            services.AddSingleton<IMessagePublisher, SerializedMessagePublisher>();
            services.AddSingleton(_ => new MessageHandlerAssemblies {typeof(Program).Assembly});
            services.AddSingleton<IRawMessageHandler, DispatchingRawMessageHandler>();
            services.AddSingleton<IMessageHandlerInvoker>(c =>
                new MessageHandlerInvoker(
                    c.GetService<IMessageHandlerInfoProvider>(),
                    c.GetService));

            services.AddSingleton(new MessageHandlerDiscoveryOptions
            {
                MessageHandlerAssemblies = {typeof(Program).Assembly},
                IncludeNonPubicHandlers = true
            });

            // options.MessageTypeTopicMap.Add(type,topic);
            // - auto discovery based on SomeMessage attributes?
            services.AddSingleton<IMessageTypeTopicMap>(c =>
                new MessageTypeTopicMap
                {
                    {typeof(SomeMessage), "SomeTopic"}
                });

            services.AddScoped<IMessageHandler<SomeMessage>, SomeMessageHandler>();
            services.AddScoped<SomeMessageHandler, SomeMessageHandler>();

            // options.AddRedis();
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6379"));
            services.AddSingleton<IRawMessagePublisher, RedisRawMessagePublisher>();
            services.AddSingleton<IRawMessageHandlerSubscriber, RedisRawMessageHandlerSubscriber>();
            services.AddSingleton<IRawMessageHandler, DispatchingRawMessageHandler>();
            services.AddSingleton<IMessageHandlerInfoProvider, MessageHandlerInfoProvider>();

            // options.AddProtobuf();
            RuntimeTypeModel.Default.Add(typeof(SomeMessage), false).Add(1, nameof(SomeMessage.Body));
            services.AddSingleton<ISerializer, ProtobufSerializer>();

            var container = services.BuildServiceProvider();

            // Configure()
            // app.AddSubscriptions()
            var map = container.GetRequiredService<IMessageTypeTopicMap>();
            var rawSubscriber = container.GetRequiredService<IRawMessageHandlerSubscriber>();
            var rawHandler = container.GetRequiredService<IRawMessageHandler>();
            foreach (var topic in map.GetTopics())
            {
                rawSubscriber.Subscribe(topic, rawHandler, None);
            }

            // Application logic
            var publisher = container.GetRequiredService<IMessagePublisher>();

            publisher.Publish(new SomeMessage {Body = "Some message body."}, None)
                .GetAwaiter()
                .GetResult();

            Console.ReadKey();
        }
    }
}