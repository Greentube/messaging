using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Meta;

namespace Greentube.Messaging.Sample.Kafka
{
    public class Program
    {
        static void Main()
        {
            using (var app = new SomeApp(s =>
                s.AddMessaging(builder => builder
                    .AddTopic<SomeMessage>("some.topic")
                    .AddTopic<SomeOtherMessage>("some.topic.other")
                    .AddKafka(o =>
                    {
                        o.Properties.BrokerList = "localhost:9092";
                        o.Properties.GroupId = "Sample-GroupId";
                        o.Properties.Add("socket.timeout.ms", 1000); // sample unmapped setting
                        o.Subscriber.ConsumerCreatedCallback =
                            consumer => consumer.OnError += (sender, error)
                                => Console.WriteLine($"Consumer error: ${error}");
                    })
                    .ConfigureOptions(o => { })
                    .AddHandlerDiscovery(d =>
                    {
                        d.IncludeNonPublic = true;
                        d.DiscoveredHandlersLifetime = ServiceLifetime.Singleton;
                        d.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                    })
                    .AddSerialization()
                        .AddProtoBuf())))
            {
                // Adds proto definition to the type (another option is to add [ProtoContract] to the class directly)
                RuntimeTypeModel.Default.Add(typeof(SomeMessage), false).Add(1, nameof(SomeMessage.Body));
                RuntimeTypeModel.Default.Add(typeof(SomeOtherMessage), false).Add(1, nameof(SomeOtherMessage.Number));

                var publisher = app.Run();
                for (int i = 0; i < 3; i++)
                {
                    publisher.Publish(new SomeOtherMessage {Number = i}, CancellationToken.None);
                }
            } // Graceful shutdown
        }
    }
}