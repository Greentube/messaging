using System;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Meta;

namespace Messaging.Sample.Kafka
{
    public class Program
    {
        static void Main()
        {
            using (var app = new SomeApp(s =>
                s.AddMessaging(builder =>
                {
                    builder.AddProtoBuf();
                    builder.AddKafka(o =>
                    {
                        o.Properties.BrokerList = "localhost:9092";
                        o.Properties.GroupId = "Sample-GroupId";
                        o.Properties.Add("socket.timeout.ms", 1000); // sample unmapped setting
                        o.Subscriber.ConsumerCreatedCallback =
                            consumer => consumer.OnError += (sender, error)
                                => Console.WriteLine($"Consumer error: ${error}");
                    });
                    builder.ConfigureOptions(o =>
                    {
                        o.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                    });
                    builder.AddHandlerDiscovery(d =>
                    {
                        d.IncludeNonPublic = true;
                        d.DiscoveredHandlersLifetime = ServiceLifetime.Singleton;
                        d.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                    });
                })))
            {
                // Adds proto definition to the type (another option is to add [ProtoContract] to the class directly)
                RuntimeTypeModel.Default.Add(typeof(SomeMessage), false).Add(1, nameof(SomeMessage.Body));

                app.Run();
            } // Graceful shutdown
        }
    }


}