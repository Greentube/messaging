using System;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Sample.Kafka
{
    public class Program
    {
        static void Main()
        {
            using (var app = new SomeApp(s =>
                s.AddMessaging(builder =>
                {
                    builder.AddJson();
                    builder.AddKafka(o =>
                    {
                        o.BrokerList = "localhost:9092";
                        o.GroupId = "GroupId";
                    });
                    builder.ConfigureOptions(o =>
                    {
                        o.DiscoveryOptions.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                        o.DiscoveryOptions.IncludeNonPubicHandlers = true;
                    });
                })))
            {

                app.Run();

                Console.ReadKey();

            } // Graceful shutdown
        }
    }
}