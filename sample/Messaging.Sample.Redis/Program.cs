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
                            o.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                        });
                        builder.AddHandlerDiscovery(d =>
                        {
                            d.IncludeNonPublic = true;
                            d.DiscoveredHandlersLifetime = ServiceLifetime.Singleton;
                            d.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
                        });
                    })
                    .AddSingleton<IConnectionMultiplexer>(_ =>
                        ConnectionMultiplexer.Connect("localhost:6379"))))
            {

                // Adds proto definition to the type (another option is to add [ProtoContract] to the class directly)
                RuntimeTypeModel.Default.Add(typeof(SomeMessage), false).Add(1, nameof(SomeMessage.Body));

                app.Run();
            } // Graceful shutdown
        }
    }

}