using System;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Meta;
using StackExchange.Redis;

namespace Messaging.Sample.Redis
{
    public class Program
    {
        static void Main()
        {
            using (var app = new SomeApp(s =>
                s.AddMessaging(builder =>
                    {
                        builder.AddProtoBuf();
                        builder.AddRedis();
                        builder.ConfigureOptions(o =>
                        {
                            o.DiscoveryOptions.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                            o.DiscoveryOptions.IncludeNonPubicHandlers = true;
                        });
                    })
                    .AddSingleton<IConnectionMultiplexer>(_ =>
                        ConnectionMultiplexer.Connect("localhost:6379"))))
            {

                // Adds proto definition to the type (another option is to add [ProtoContract] to the class directly)
                RuntimeTypeModel.Default.Add(typeof(SomeMessage), false).Add(1, nameof(SomeMessage.Body));

                app.Run();

                Console.ReadKey();

            } // Graceful shutdown
        }
    }
}